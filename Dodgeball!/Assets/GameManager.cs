using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // storing all AI controllers
    [SerializeField] private GameObject _plane;
    private AIAgentController[] _agentControllers;

    // ball storage and randomization variables
    public GameObject[] balls;
    [SerializeField] private float zMax;
    [SerializeField] private float xMax;

	// Use this for initialization
	void Start () {
        _agentControllers = _plane.GetComponents<AIAgentController>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartSimulation()
    {
        for(int i = 0; i < _agentControllers.Length; i++)
        {
            RandomizeBallLocations();
            _agentControllers[i].ToggleGame();
        }
    }

    private void RandomizeBallLocations()
    {
        foreach(GameObject ball in balls)
        {
            float xValue = Random.Range(-xMax, xMax);
            float zValue = Random.Range(-zMax, zMax);
            ball.transform.position = new Vector3(xValue, ball.transform.position.y, zValue);
        }
    }
}
