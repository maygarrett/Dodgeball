using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgentController : MonoBehaviour {

    //scan for specified tags within a distance
    //if something is there, move towards it

    public Agent m_agent = null;

    public float m_maxAngularSpeedAngle = 45.0f;
    public float m_minAngularSpeedAngle = 10.0f;

    public float m_maxSpeedDistance = 1.0f;
    public float m_destinationBuffer = 2.0f;
    public float m_scanDistance = 10.0f;

    // list of vector3 pathnodes
    private List<Vector3> m_pathList = new List<Vector3>();

    private bool m_isRunning = false;

    // ball throwing variables
    [SerializeField] private GameObject _currentBallTarget;
    [SerializeField] private bool _isHoldingBall;
    [SerializeField] private Transform _holdPosition;
    [SerializeField] private Transform _secondaryThrowPosition;
    [SerializeField] private GameObject[] _enemies;
    private GameObject _enemyTarget;
    [SerializeField] private float m_enemyDestinationBuffer = 75.0f;
    public float m_enemyScanDistance = 300.0f;
    [SerializeField] private GameObject _rayPosition;

    // wandering variables
    private Vector3 randomLocation;
    [SerializeField] private float blueXMax;
    [SerializeField] private float blueXMin;
    [SerializeField] private float blueZMax;
    [SerializeField] private float blueZMin;
    [SerializeField] private float redXMax;
    [SerializeField] private float redXMin;
    [SerializeField] private float redZMax;
    [SerializeField] private float redZMin;
    [SerializeField] private bool _isBlue;

    // Agent Tracking variables
    [SerializeField] private float _agentDistanceBuffer;
    [SerializeField] private float _agentXBuffer;
    private bool _isNewTracking = false;
    private float _closerEnemyDestinationBuffer = 5.0f;

    // teammates
    [SerializeField] private GameObject[] _teammates;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // activate
        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_isRunning = !m_isRunning;
        }

        //scanning and moving
        if(m_isRunning && !_isHoldingBall)
        {
            ScanForObjects();
            MoveTowardsDestination();
        }
        // stopping everything
        else if(!m_isRunning && !_isHoldingBall)
        {
            m_agent.StopAngularVelocity();
            m_agent.StopLinearVelocity();
        }
        // holding a ball (attack state)
        else if(_isHoldingBall && m_isRunning)
        {
            // do holding ball behaviour
            HoldingBallBehaviour();
            //ScanForEnemy();
            MoveTowardsEnemy();

        }
    }

    bool HasDestination()
    {
        // if there are destinations in the path list
        if(m_pathList.Count > 0)
        {
            // set destination to the next path node, calculate speed stuff and once destination is reached delete node from list
            Vector3 destination = m_pathList[0];

            Vector3 toDestination = destination - m_agent.transform.position;
            float distanceToDestination = toDestination.magnitude;
            if(distanceToDestination < m_destinationBuffer)
            {
                if (_currentBallTarget)
                {
                    if (Vector3.Distance(_currentBallTarget.transform.position, destination) < m_destinationBuffer)
                    {
                        PickUpBall();
                    }
                }

                m_pathList.RemoveAt(0);
            }
        }

        // return true if there are still pathnodes in the list
        return m_pathList.Count > 0;
    }

    float CalculateConsiderationValue(float val, float min, float max )
    {
        float range = max - min;
        float value = Mathf.Clamp(val, min, max);
        float considerationValue = (value - min) / range;
        return considerationValue;
    }

    float CalculateConsiderationUtil(List<float> considerationList)
    {
        float numConsiderations = (float)considerationList.Count;
        float finalScore = numConsiderations > 0.0f ? 1.0f : 0.0f;
        foreach (float considerationScore in considerationList)
        {
            float modificationFactor = 1.0f - (1.0f / numConsiderations);
            float makeupValue = (1.0f - considerationScore) * modificationFactor;
            finalScore *= considerationScore + (makeupValue * considerationScore);
        }
        return finalScore;
    }

    // move agent towards destination
    void MoveTowardsDestination()
    {
        // if there are no more path nodes, stop moving
        if(!HasDestination())
        {
            m_agent.StopAngularVelocity();
            return;
        }

        // calculate all movement and speeds angles etc for moving towards destination
        Vector3 destination = m_pathList[0];
        if(_currentBallTarget)
        {
            if(_currentBallTarget.GetComponent<BallProjectile>().GetIsHeld())
            {
                ResetDestinations();
                return;
            }

            destination = _currentBallTarget.transform.position;
        }

        // check if destination is on correct side of court
        if (_isBlue)
        {
            if (destination.z < 0)
            {
                _currentBallTarget = null;
                m_pathList.Clear();
                destination = GenerateRandomLocation(_isBlue);
                m_pathList.Add(destination);
            }
        }
        else
        {
            if (destination.z > 0)
            {
                _currentBallTarget = null;
                m_pathList.Clear();
                destination = GenerateRandomLocation(_isBlue);
                m_pathList.Add(destination);
            }
        }

        Vector3 toDestination = destination - m_agent.transform.position;
        float distanceToDestination = toDestination.magnitude;
        toDestination.Normalize();
        float lookAtToDestinationDot = Vector3.Dot(m_agent.transform.forward, toDestination);
        float rightToDestinationDot = Vector3.Dot(m_agent.transform.right, toDestination);
        float toDestinationAngle = Mathf.Rad2Deg * Mathf.Acos(lookAtToDestinationDot);
        
        // generate list of speed considerations to pass to calculate functions
        List<float> speedConsiderations = new List<float>();

        float distanceConsideration = CalculateConsiderationValue(distanceToDestination,m_destinationBuffer, m_maxSpeedDistance);
        float angleConsideration = CalculateConsiderationValue(toDestinationAngle, m_minAngularSpeedAngle, m_maxAngularSpeedAngle);
        float speedAngleConsideration = 1.0f - angleConsideration;
        speedConsiderations.Add(distanceConsideration);
        speedConsiderations.Add(speedAngleConsideration);

        // set the agents speed and angular speed based on list of considerations generated earlier
        float speed = CalculateConsiderationUtil(speedConsiderations) * m_agent.m_linearMaxSpeed;
        m_agent.linearSpeed = speed;
        float angularSpeed = angleConsideration * m_agent.m_angularMaxSpeed;
        m_agent.angularSpeed = angularSpeed;

        //how do we face our destination
        bool shouldTurnRight = rightToDestinationDot > Mathf.Epsilon;
        if (shouldTurnRight)
        {
            m_agent.TurnRight();
        }
        else
        {
            m_agent.TurnLeft();
        }
        

        if (distanceToDestination > m_destinationBuffer)
        {
            m_agent.MoveForwards();
        }
        else if (_currentBallTarget)
        {
            PickUpBall();
        }
    }

    // function draws out a path for the agent using navmesh
    void OnDestinationFound(Vector3 destination)
    {
        //Debug.Log("OnDestinationFound() called");
        NavMeshPath path = new NavMeshPath();
        bool isSuccess = NavMesh.CalculatePath(m_agent.transform.position, destination, NavMesh.AllAreas, path);
        if(isSuccess)
        {
            //draw out the path
            //set the destination
            foreach(Vector3 pathNode in path.corners)
            {
                m_pathList.Add(pathNode);
                // Debug.Log("Path Pos: " + pathNode);
            }

            foreach (Vector3 dest in m_pathList)
            {
                //Debug.Log("List Pos: " + dest);
            }
        }
    }

    void ScanForObjects()
    {
        // Debug.Log("agent scanning for objects");

        //don't scan when we have somewhere to go
        if(HasDestination())
        {
            return;
        }


        // finding objects marked with the layer "Interactable"
        Vector3 agentPosition = m_agent.transform.position;
        int layer = LayerMask.NameToLayer("Interactable");
        int layerMask = 1 << layer;
        Collider[] hitColliders = Physics.OverlapSphere(agentPosition, m_scanDistance, layerMask);
        if (hitColliders.Length == 0)
        {
            // add a random location to OnDestinationFound
            
            Vector3 randomLocation = GenerateRandomLocation(_isBlue);
            OnDestinationFound(randomLocation);
        }
        else
        {
            if(m_pathList.Count > 0)
            {
                m_pathList.Clear();
            }

            foreach (Collider hitCollider in hitColliders)
            {

                float distanceToObject = Vector3.Distance(agentPosition, hitCollider.transform.position);
                if (distanceToObject < m_destinationBuffer)
                {
                    continue;
                }

                if (hitCollider.gameObject.tag == "Ball")
                {
                    if (!hitCollider.gameObject.GetComponent<BallProjectile>().GetIsHeld())
                    {
                        _currentBallTarget = hitCollider.gameObject;

                    }
                    if (_currentBallTarget)
                    {
                        OnDestinationFound(hitCollider.transform.position);
                    }
                    else
                    {
                        Vector3 randomLocation = GenerateRandomLocation(_isBlue);
                        OnDestinationFound(randomLocation);
                    }

                    break;
                }
            }
        }
    }

    void PickUpBall()
    {
        if (!_currentBallTarget.GetComponent<BallProjectile>().GetIsHeld())
        {
            _currentBallTarget.GetComponent<BallProjectile>().SetIsHeld(true);
            Debug.Log("Picking up ball");
            _isHoldingBall = true;
        }
    }

    void ThrowBall()
    {
        if (_enemyTarget && _isHoldingBall)
        {
            Debug.Log("Throwing Ball");
            _currentBallTarget.GetComponent<BallProjectile>().SetIsHeld(false);
            float accuracy = Random.Range(0.85f, 1.15f);
            _currentBallTarget.GetComponent<BallProjectile>().ThrowBall(_enemyTarget.gameObject.GetComponent<BasicVelocity>(), accuracy);
            _isHoldingBall = false;
            _currentBallTarget = null;
            //m_pathList.Clear();
            //StartCoroutine(Wait(2));
            //ScanForObjects();
        }
    }

    /*
    void ScanForEnemy()
    {
        int layer = LayerMask.NameToLayer("Agent");
        int layerMask = 1 << layer;
        Collider[] hitColliders = Physics.OverlapSphere(m_agent.transform.position, m_enemyScanDistance, layerMask);
        foreach(Collider Target in hitColliders)
        {
            if(Target != gameObject.GetComponent<Collider>())
            {
                _enemyTarget = Target.gameObject;
                _currentBallTarget.gameObject.GetComponent<BallProjectile>().m_movingTarget = _enemyTarget.gameObject.GetComponent<BasicVelocity>();
            }
            else
            {
                /_enemyTarget = GameObject.FindGameObjectWithTag("Agent");
                _currentBallTarget.gameObject.GetComponent<BallProjectile>().m_movingTarget = _enemyTarget.gameObject.GetComponent<BasicVelocity>();
            }
        }
    
    }*/


    void MoveTowardsEnemy()
    {
        if (_isHoldingBall)
        {

            Vector3 destination = _enemyTarget.transform.position;

            /*// account for issues with crossing line while seeking enemy
            if (Mathf.Abs(_enemyTarget.transform.position.x - gameObject.transform.position.x) > _agentXBuffer)
            {
                _isNewTracking = true;

                if (_isBlue)
                {
                    destination = new Vector3(_enemyTarget.transform.position.x, _enemyTarget.transform.position.y, 10.0f);
                }
                else
                {
                    destination = new Vector3(_enemyTarget.transform.position.x, _enemyTarget.transform.position.y, -10.0f);
                }
            }
            else
            {
                _isNewTracking = false;
            }*/
            
            Vector3 toDestination = destination - m_agent.transform.position;
            float distanceToDestination = toDestination.magnitude;
            toDestination.Normalize();
            float lookAtToDestinationDot = Vector3.Dot(m_agent.transform.forward, toDestination);
            float rightToDestinationDot = Vector3.Dot(m_agent.transform.right, toDestination);
            float toDestinationAngle = Mathf.Rad2Deg * Mathf.Acos(lookAtToDestinationDot);

            // generate list of speed considerations to pass to calculate functions
            List<float> speedConsiderations = new List<float>();

            float distanceConsideration = CalculateConsiderationValue(distanceToDestination, m_enemyDestinationBuffer, m_maxSpeedDistance);
            float angleConsideration = CalculateConsiderationValue(toDestinationAngle, m_minAngularSpeedAngle, m_maxAngularSpeedAngle);
            float speedAngleConsideration = 1.0f - angleConsideration;
            speedConsiderations.Add(distanceConsideration);
            speedConsiderations.Add(speedAngleConsideration);

            // set the agents speed and angular speed based on list of considerations generated earlier
            float speed = CalculateConsiderationUtil(speedConsiderations) * m_agent.m_linearMaxSpeed;
            m_agent.linearSpeed = speed;
            float angularSpeed = angleConsideration * m_agent.m_angularMaxSpeed;
            m_agent.angularSpeed = angularSpeed;

            //how do we face our destination
            bool shouldTurnRight = rightToDestinationDot > Mathf.Epsilon;
            if (shouldTurnRight)
            {
                m_agent.TurnRight();
            }
            else
            {
                m_agent.TurnLeft();
            }

            float buffer = m_enemyDestinationBuffer;

            /*if(_isNewTracking)
            {
                buffer = _closerEnemyDestinationBuffer;
            }*/

            if (distanceToDestination > buffer)
            {
                m_agent.MoveForwards();
            }
            else
            {
                ThrowBall();
            }
        }
    }

    Vector3 GenerateRandomLocation(bool isBlue)
    {
        float xValue;
        float yValue;
        float zValue;

        if (isBlue)
        {
            xValue = Random.Range(blueXMin, blueXMax);
            yValue = gameObject.transform.position.y;
            zValue = Random.Range(blueZMin, blueZMax);
        }
        else
        {
            xValue = Random.Range(redXMin, redXMax);
            yValue = gameObject.transform.position.y;
            zValue = Random.Range(redZMin, redZMax);
        }

        return new Vector3(xValue, yValue, zValue);
    }

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private void HoldingBallBehaviour()
    {
        EnemyDecider();

        // if holdposition is closer
        if(Vector3.Distance(_holdPosition.position, _enemyTarget.transform.position) < Vector3.Distance(_secondaryThrowPosition.position, _enemyTarget.transform.position))
        {
            // hold ball in hold position
            _currentBallTarget.transform.position = _holdPosition.position;
        }
        else
        {
            // hold ball in secondary position
            _currentBallTarget.transform.position = _secondaryThrowPosition.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ThrowCollider")
        {
            if(_isHoldingBall)
            {
                ThrowBall();
            }
        }
    }

    private void EnemyDecider()
    {
        foreach(GameObject enemy in _enemies)
        {
            if(_enemyTarget == null)
            {
                _enemyTarget = enemy;
            }

            if(Vector3.Distance(gameObject.transform.position, _enemyTarget.transform.position) > Vector3.Distance(gameObject.transform.position, enemy.transform.position))
            {
                _enemyTarget = enemy;
            }
        }
    }

    private void ResetDestinations()
    {
        m_pathList.Clear();
        _currentBallTarget = null;
    }

}

