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

    public List<GameObject> getPlayers() {
        return Players;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [SerializeField]
    public GameState GameState { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameState = new GameState();
            GameState.Players = new List<GameObject>();
            GameState.HighScores = new List<int>();
            GameState.DifficultyLevel = 1;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
       
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

}
