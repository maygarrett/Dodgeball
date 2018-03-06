using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicVelocity : MonoBehaviour {

    public Vector3 m_velocity;
    private Rigidbody m_rb = null;
	// Use this for initialization
	void Start () {
        m_rb = GetComponent<Rigidbody>();
	}

    public Vector3 GetVelocity() { return m_rb.velocity; }

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_rb.velocity = m_velocity;
        }
	}
}
