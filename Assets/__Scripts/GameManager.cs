using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

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

    // seed 
    public int Seed { get; set; }
}

public class GameManager : MonoBehaviour
{


    [SerializeField]
    public GameState GameState { get; set; }
    public static GameManager Instance { get; private set; }

    // get function will get it form game state
    public int Seed { get { return GameState.Seed; }  }

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance != this)
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
            GameState.Seed = Random.Range(0, 42);
            DontDestroyOnLoad(gameObject);
        }
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
        if (GameState == null) return;
        GameState.Players = new List<GameObject>();
        Debug.Log("Heavy load of Player");
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

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Restart();
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

    public void Restart()
    {
        Debug.Log("Restarting");
        SceneManager.LoadScene(0);
        GameState.Seed = Random.Range(0, 42) + GameState.Seed;
        LevelManager.Instance?.Regenerate();
    }  

    public void OnPlayerDied(GameObject player)
    {
        // move player to last room
        GameState.Players.Remove(player);

    }


}
