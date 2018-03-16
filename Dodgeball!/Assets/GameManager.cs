using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // storing all AI controllers
    [SerializeField] private GameObject _plane;
    private AIAgentController[] _agentControllers;

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
            _agentControllers[i].ToggleGame();
        }
    }
}
