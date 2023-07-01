using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;
    public static LevelManager Instance { get { return instance; } }

 void Awake()
    {
        // Singleton pattern: Initialize the static instance on Awake
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        watcher = new Watcher();
        voidWatcher = new Watcher();
        if (gameObject.GetComponent<RoomController>() == null)
            controller = gameObject.AddComponent<RoomController>();
        else
            controller = gameObject.GetComponent<RoomController>();
    }

    public RoomInternal catalog;

    private void Start()
    {
        // Initialize the LevelController and RoomManager
        LevelController levelController = FindObjectOfType<LevelController>();
        if (levelController == null)
        {
            GameObject levelControllerGO = new GameObject("LevelController");
            levelController = levelControllerGO.AddComponent<LevelController>();
        }

        levelController.catalog = catalog; // Set catalog to be accessible
    }
}
