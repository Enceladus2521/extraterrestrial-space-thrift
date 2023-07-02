using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameState
{
    public List<GameObject> Players { get; set; }

    public List<GameObject> DeadPlayers { get; set; }

    public int Difficulty { get; set; }

    public List<int> HighScores { get; set; }

    public int Seed { get; set; }
}

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private RoomInternal catalog;
    public RoomInternal Catalog { get { return catalog; } }

    [SerializeField]
    private GameState gameState;
    public GameState GameState { get { return gameState; } }
    public static GameManager Instance { get; private set; }

    // get function will get it form game state
    public int Seed { get { return GameState.Seed; } }

    public List<GameObject> Players { get { return GameState.Players; } }

    private void Awake()
    {

        if (GameManager.Instance != null && GameManager.Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            gameState = new GameState();
            gameState.Players = new List<GameObject>();
            gameState.DeadPlayers = new List<GameObject>();
            gameState.HighScores = new List<int>();
            gameState.Difficulty = 1;
            gameState.Seed = Random.Range(0, 42);
            DontDestroyOnLoad(gameObject);
        }
    }



    private void Start()
    {
        Application.targetFrameRate = 144;
        Restart();
    }



    // check all players by tag and update
    public void UpdatePlayers()
    {
        if (GameState == null) return;
        GameState.Players = GameObject.FindGameObjectsWithTag("Player").ToList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AddHighScore(LevelManager.Instance.GetTopDifficulty());
            Restart();
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


    public int GetHighestScore()
    {
        int max = -1;
        for (int i = 0; i < GameState.HighScores.Count; i++)
        {
            if (GameState.HighScores[i] > max)
            {
                max = GameState.HighScores[i];
            }
        }

        return max;
    }


    private void OnDestroy()
    {
        if (this == Instance) { Instance = null; }
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
        GameState.Players.Clear();
        GameState.DeadPlayers.Clear();

        GameState.Seed = Random.Range(0, 42) + GameState.Seed;

        Debug.Log("Seed: " + GameState.Seed);
    }

    public void OnPlayerDied(GameObject player)
    {

        GameState.DeadPlayers.Add(player);
        GameState.Players.Remove(player);
        if (GameState.Players.Count == 0)
        {
            AddHighScore(LevelManager.Instance.GetTopDifficulty());

            Restart();
        }

    }

    public void OnRoomFinished(RoomManager roomManager)
    {
        Vector3 roomCenter = roomManager.GetRoomCenter();
        for (int i = 0; i < GameState.DeadPlayers.Count; i++)
        {
            GameObject player = GameState.DeadPlayers[i];
            player.transform.position = roomCenter;
            GameState.Players.Add(player);
            GameState.DeadPlayers.Remove(player);
            player.SetActive(true);
            // player.GetComponent<PlayerStats>().SetStats();

        }

    }

}