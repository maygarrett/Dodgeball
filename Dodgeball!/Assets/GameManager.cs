using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    // game over stuff
    [SerializeField] private Canvas _gameOverCanvas;
    [SerializeField] private Text _winnerText;

    // player or simulation toggle stuff
    [SerializeField] private GameObject _simulationCamera;
    [SerializeField] private GameObject _playerCamera;
    [SerializeField] private GameObject _player;
    private PlayerController _playerController;
    private AIAgentController _playerAgentController;

    // Use this for initialization
    void Start() {
        _agentControllers = _plane.GetComponents<AIAgentController>();

        _playerController = _player.GetComponent<PlayerController>();
        _playerAgentController = GetPlayerAgentController();


    }

    // Update is called once per frame
    void Update() {

        if (gameOn)
        {
            // do game on stuff
        }
    }

    public void StartSimulation()
    {
        gameOn = true;
        RandomizeBallLocations();

        // make switches that stop player control
        _simulationCamera.SetActive(true);
        _playerCamera.SetActive(false);
        _playerController.enabled = false;
        _playerAgentController.enabled = true;
        for (int i = 0; i < _agentControllers.Length; i++)
        {
            _agentControllers[i].ToggleGame();
        }
        _menuCanvas.enabled = false;
    }

    public void StartPlayerGame()
    {
        gameOn = true;
        RandomizeBallLocations();

        // make switches that allow for player control
        _simulationCamera.SetActive(false);
        _playerCamera.SetActive(true);
        _playerController.enabled = true;
        _playerAgentController.enabled = false;
        for (int i = 0; i < _agentControllers.Length; i++)
        {
            _agentControllers[i].ToggleGame();
        }
        _menuCanvas.enabled = false;
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

    public void CheckPlayers(AIAgentController mostRecentElimination)
    {
        Debug.Log("checking players to see if alive");
        int blueTeam = 0;
        int redTeam = 0;

        Agent[] players = FindObjectsOfType<Agent>();
        Debug.Log("found " + players.Length + " players");
        foreach (Agent player in players)
        {
            if(player.gameObject == mostRecentElimination.m_agent.gameObject)
            {
                continue;
            }

            Debug.Log("player " + player.gameObject.name + " is still alive");

            if (player._isBlue)
            {
                blueTeam++;
                Debug.Log("adding one to blue team");
            }
            else
            {
                redTeam++;
                Debug.Log("adding one to red team");
            }
        }

        if(blueTeam == 0)
        {
            Debug.Log("red team wins");
            string winner = "Red Team Wins";
            GameOver(winner);
        }
        else if(redTeam == 0)
        {
            Debug.Log("blue team wins");
            string winner = "Blue Team Wins";
            GameOver(winner);
        }
    }

    private void GameOver(string winner)
    {
        Debug.Log("Game is Over");
        gameOn = false;
        _gameOverCanvas.gameObject.SetActive(true);
        _winnerText.text = winner;
        //Time.timeScale = 0;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    private AIAgentController GetPlayerAgentController()
    {
        AIAgentController[] players = _plane.GetComponents<AIAgentController>();

        AIAgentController playerAgent = null;

        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].m_agent.gameObject == _player)
            {
                playerAgent = players[i];
            }
        }

        return playerAgent;
    }
}
