using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Agent m_agent = null;

    // pick up variables
    [SerializeField] private Transform _holdBallLocation;
    [SerializeField] private float _pickUpDistance;
    private bool _isHolding = false;
    private GameObject[] _balls;
    private GameObject _heldBall;

    // enemy variables
    [SerializeField] private GameObject[] _enemies;
    private GameObject _enemyTarget;


	// Use this for initialization
	void Start () {

        _balls = GameObject.FindGameObjectsWithTag("Ball");
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_agent)
        {
            return;
        }

        bool isLinearIdle = true;
        bool isAngularIdle = true;

        // movement inputs
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            isLinearIdle = false;
            m_agent.StrafeRight();
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            isLinearIdle = false;
            m_agent.StrafeLeft();
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            isLinearIdle = false;
            m_agent.MoveForwards();
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            isLinearIdle = false;
            m_agent.MoveBackwards();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            isAngularIdle = false;
            m_agent.TurnLeft();
        }
        if (Input.GetKey(KeyCode.E))
        {
            isAngularIdle = false;
            m_agent.TurnRight();
        }
        if(isLinearIdle)
        {
            m_agent.StopLinearVelocity();
        }
        if(isAngularIdle)
        {
            m_agent.StopAngularVelocity();
        }

        // pick up ball check
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            // user presses space and is not holding a ball, look to pick up ball
            if (!_isHolding)
            {
                if(_balls.Length == 0)
                {
                    _balls = GameObject.FindGameObjectsWithTag("Ball");
                }

                foreach (GameObject ball in _balls)
                {
                    if (Vector3.Distance(gameObject.transform.position, ball.transform.position) < _pickUpDistance)
                    {
                        _heldBall = ball;
                        _isHolding = true;
                        // return;
                    }
                }
            }
            // user presses space and is holding a ball
            else
            {
                // start throw behaviour
                _heldBall.GetComponent<BallProjectile>().ThrowBall(_enemyTarget.gameObject.GetComponent<BasicVelocity>(), 1);
                _isHolding = false;
                _heldBall = null;
                _enemyTarget = null;
            }
        }

        // holding ball behaviour
        if(_isHolding)
        {
            EnemyDecider();
            _heldBall.transform.position = _holdBallLocation.transform.position;
        }
    }

    private void EnemyDecider()
    {
        foreach (GameObject enemy in _enemies)
        {
            if (enemy == null)
            { continue; }
            if (_enemyTarget == null)
            { _enemyTarget = enemy; }
            if (Vector3.Distance(gameObject.transform.position, _enemyTarget.transform.position) > Vector3.Distance(gameObject.transform.position, enemy.transform.position))
            { _enemyTarget = enemy; }
        }
    }

    private void OnDestroy()
    {
        GameObject.FindObjectOfType<GameManager>().PlayerEliminated();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerDeath")
        {
            Destroy(this.gameObject);
        }
    }

}
