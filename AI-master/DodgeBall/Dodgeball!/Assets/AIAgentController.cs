using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgentController : MonoBehaviour {

    //scan for specified tags within a distance
    //if something is there, move towards it

    public Agent m_agent = null;

    public float m_destinationBuffer = 2.0f;

    private Vector3 m_destination = Vector3.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ScanForObjects();
        MoveTowardsDestination();
    }

    bool HasDestination()
    {
        return m_destination != Vector3.zero;
    }

    void MoveTowardsDestination()
    {
        if(!HasDestination())
        {
            return;
        }

        Vector3 lookAt = m_agent.transform.forward;
        lookAt.Normalize();
        Vector3 toDestination = m_destination - m_agent.transform.position;
        float distanceToDestination = toDestination.magnitude;
        toDestination.Normalize();
        float toDestinationDot = Vector3.Dot(lookAt, toDestination);
        Debug.Log("Distance: " + distanceToDestination);
        Debug.Log("Dot: " + toDestinationDot);
        float toDestinationAngle = Mathf.Rad2Deg * Mathf.Acos(toDestinationDot);
        Debug.Log("Angle: " + toDestinationAngle);

        //turn if angle is greater than some threshold
        //move until close to target

    }

    void ScanForObjects()
    {
        //don't scan when we have somewhere to go
        if(HasDestination())
        {
            return;
        }

        Vector3 agentPosition = m_agent.transform.position;
        int layer = LayerMask.NameToLayer("Interactable");
        int layerMask = 1 << layer;
        Collider[] hitColliders = Physics.OverlapSphere(agentPosition, 10.0f, layerMask);
        foreach(Collider hitCollider in hitColliders)
        {
            float distanceToObject = Vector3.Distance(agentPosition, hitCollider.transform.position);
            if(distanceToObject < m_destinationBuffer)
            {
                continue;
            }

            m_destination = hitCollider.transform.position;
            break;
        }
    }


}
