using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // storing UI and Hud Canvas'
    [SerializeField] private Canvas _menuCanvas;
    [SerializeField] private Canvas _gameHUD;

    // storing all AI controllers
    [SerializeField] private GameObject _plane;
    private AIAgentController[] _agentControllers;

    // storing Agent Objects
    [SerializeField] private GameObject[] _blueTeam;
    [SerializeField] private GameObject[] _redTeam;

    // ball storage and randomization variables
    public GameObject[] balls;
    [SerializeField] private float zMax;
    [SerializeField] private float xMax;
    [SerializeField] private float zMin;
    [SerializeField] private float xMin;

    // game variables
    public bool gameOn = false;

	// Use this for initialization
	void Start () {
        _agentControllers = _plane.GetComponents<AIAgentController>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(gameOn)
        {
            CheckBlueTeam();
            CheckRedTeam();
        }
	}

    public void StartSimulation()
    {
        for(int i = 0; i < _agentControllers.Length; i++)
        {
            gameOn = true;
            RandomizeBallLocations();
            _agentControllers[i].ToggleGame();
            _menuCanvas.enabled = false;
        }
    }

    private void RandomizeBallLocations()
    {
        foreach(GameObject ball in balls)
        {
            float xValue = Random.Range(xMin, xMax);
            float zValue = Random.Range(zMin, zMax);

            int someValue = Random.Range(0, 2) * 2 - 1;
            int someValue2 = Random.Range(0, 2) * 2 - 1;

            xValue = xValue * someValue;
            zValue = zValue * someValue2;

            ball.transform.position = new Vector3(xValue, ball.transform.position.y, zValue);
        }
    }

    private void CheckBlueTeam()
    {
        int playersLeft = _redTeam.Length;
        foreach(GameObject player in _redTeam)
        {
            if(player == null)
            {
                playersLeft--;
            }
        }

        if(playersLeft <= 0)
        {
            GameOver();
        }
    }

    private void CheckRedTeam()
    {
        int playersLeft = _redTeam.Length;
        foreach (GameObject player in _blueTeam)
        {
            if (player == null)
            {
                playersLeft--;
            }
        }

        if (playersLeft <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        gameOn = false;
        Time.timeScale = 0;
    }
}
