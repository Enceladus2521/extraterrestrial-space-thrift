using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class GameState
{
    [SerializeField]
    public List<GameObject> Players { get; set; }
    [SerializeField]
    public int DifficultyLevel { get; set; }
    [SerializeField]
    public List<int> HighScores { get; set; }

    public List<GameObject> getPlayers()
    {
        return Players;
    }
}

public class GameManager : MonoBehaviour
{

    [SerializeField]
    public GameState GameState { get; set; }
    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            GameState = new GameState();
            GameState.Players = new List<GameObject>();
            GameState.HighScores = new List<int>();
            GameState.DifficultyLevel = 1;
            DontDestroyOnLoad(gameObject);
        }
        UpdatePlayers();
    }



    private void Start()
    {
        Application.targetFrameRate = 144;
        UpdatePlayers();
    }

    void OnValidate()
    {
        UpdatePlayers();
    }


    // check all players by tag and update 
    public void UpdatePlayers()
    {   
        if(GameState == null) return;
        GameState.Players = new List<GameObject>();
        GameState.Players = GameObject.FindGameObjectsWithTag("Player").ToList();
   
    }
    public void AddPlayer(GameObject player)
    {
        GameState.Players.Add(player);
    }

    public void RemovePlayer(GameObject player)
    {
        GameState.Players.Remove(player);
    }

    private void Update()
    {
        //if Escape is pressed, quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }
    public void AddHighScore(int score)
    {
        GameState.HighScores.Add(score);
        string highScoresString = string.Join(",", GameState.HighScores.Select(i => i.ToString()).ToArray());
        PlayerPrefs.SetString("highScores", highScoresString);
        PlayerPrefs.Save();
    }

    public void LoadHighScores()
    {
        string highScoresString = PlayerPrefs.GetString("highScores");
        GameState.HighScores = highScoresString.Split(',').Select(int.Parse).ToList();
    }
    private void OnDestroy()
    {
        if (this == Instance) { Instance = null; }
    }


}
