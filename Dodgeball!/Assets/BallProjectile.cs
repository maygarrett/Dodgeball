using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProjectile : MonoBehaviour {

    public bool m_isRunning = false;
    private Rigidbody m_rb = null;

    public Vector3 m_initialVelocity = Vector3.zero;
    private float m_timeElapsed = 0.0f;

    public Transform m_targetTransform;
    public float m_minSpeed;

    public BasicVelocity m_movingTarget = null;
    public float m_desiredAirTime = 1.0f;

	// Use this for initialization
	void Start () {
        m_rb = GetComponent<Rigidbody>();
	}

    Vector3 CalculateInitialVelocityMovingTarget()
    {
        //find out where the target will be in our desired time
        //aim for that position
        Vector3 targetVelocity = m_movingTarget.GetVelocity();
        Vector3 targetDisplacement = targetVelocity * m_desiredAirTime;
        Vector3 targetPosition = m_movingTarget.transform.position + targetDisplacement;
        return CalculateInitialVelocity(targetPosition,true);
    }

    Vector3 CalculateInitialVelocity(Vector3 targetPosition,bool useDesiredTime)
    {
        Vector3 displacement = targetPosition - this.transform.position;
        float yDisplacement = displacement.y;
        displacement.y = 0.0f;
        float horizontalDisplacement = displacement.magnitude;
        if(horizontalDisplacement < Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        //v = d/t
        //vt = d
        //t = d/v

        float horizontalSpeed = useDesiredTime ? horizontalDisplacement / m_desiredAirTime : m_minSpeed;

        float time = horizontalDisplacement / horizontalSpeed;
        //we know the time it requires to reach the target
        //we need the initial velocity, that can ensure the
        //projectile gets airborn for half that time
        //1/2 ascending 1/2 descend 
        time *= 0.5f;
        //a = v/t
        //at = v
        //v is delta velocity, Vf - Vi
        //final velocity is 0, it is the peak of our upward travel

        //-Vi = at
        //Vi = -at
        Vector3 initialYVelocity = Physics.gravity * time * -1.0f;
        //assuming min velocity is a flat vector
        displacement.Normalize();
        Vector3 initialHorizontalVelocity = displacement * horizontalSpeed;
        return initialHorizontalVelocity + initialYVelocity;
    }

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            m_isRunning = !m_isRunning;
            //m_rb.velocity = CalculateInitialVelocity(m_targetTransform.position,false);
            m_rb.velocity = CalculateInitialVelocityMovingTarget();
        }

        //m_rb.useGravity = m_isRunning;

        if(m_rb.velocity.magnitude > 0.0001f)
        {
            m_timeElapsed += Time.deltaTime;
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Time Elapsed: " + m_timeElapsed);
    }
}
