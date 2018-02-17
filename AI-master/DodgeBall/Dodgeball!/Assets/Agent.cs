using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    public Transform m_redPost;
    public Transform m_bluePost;

    public Material m_redMaterial;
    public Material m_blueMaterial;

    public Material m_defaultMaterial;

    public float m_linearMaxSpeed = 0.0f;
    public float m_angularMaxSpeed = 0.0f;

    public float m_linearAcceleration = 0.0f;
    public float m_angularAcceleration = 0.0f;

    private Rigidbody m_rb = null;

    // Use this for initialization
    void Start () {
        m_rb = GetComponent<Rigidbody>();
        DetermineAgentColour();
    }
	
	// Update is called once per frame
	void Update () {
        DetermineAgentColour();
    }

    protected void DetermineAgentColour()
    {
        Vector3 agentPosition = transform.position;
        float distanceToRedPost = Vector3.Distance(agentPosition, m_redPost.position);
        float distanceToBluePost = Vector3.Distance(agentPosition, m_bluePost.position);
        if( Mathf.Abs(distanceToBluePost - distanceToRedPost) < 5.0f )
        {
            //neutral
            SetMaterial(m_defaultMaterial);

        }
        else if( distanceToRedPost > distanceToBluePost )
        {
            //blue
            SetMaterial(m_blueMaterial);
        }
        else
        {
            //red
            SetMaterial(m_redMaterial);
        }
    }

    protected void SetMaterial(Material mat)
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.material = mat;
    }

    public void MoveForwards()
    {
        m_rb.velocity = transform.forward * m_linearMaxSpeed;
    }

    public void MoveBackwards()
    {
        m_rb.velocity = transform.forward * m_linearMaxSpeed * -1.0f;
    }

    public void StrafeLeft()
    {
        m_rb.velocity = transform.right * m_linearMaxSpeed * -1.0f;
    }

    public void StrafeRight()
    {
        m_rb.velocity = transform.right * m_linearMaxSpeed;
    }

    public void TurnRight()
    {
        m_rb.angularVelocity = transform.up * m_angularMaxSpeed;
    }

    public void TurnLeft()
    {
        m_rb.angularVelocity = transform.up * m_angularMaxSpeed * -1.0f;
    }

    public void StopLinearVelocity()
    {
        Vector3 stopLinearVelocity = m_rb.velocity;
        stopLinearVelocity.x = 0.0f;
        stopLinearVelocity.z = 0.0f;
        m_rb.velocity = stopLinearVelocity;
    }

    public void StopAngularVelocity()
    {
        m_rb.angularVelocity = Vector3.zero;
    }
}
