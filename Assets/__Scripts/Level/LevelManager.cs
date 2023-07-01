using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private float roomGenerationDistance = 0.5f;
    private static LevelManager instance;
    public static LevelManager Instance { get { return instance; } }

    private LevelController controller;
    private Watcher watcher;
    private Watcher voidWatcher;

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
        if (gameObject.GetComponent<LevelController>() == null)
            controller = gameObject.AddComponent<LevelController>();
        else
            controller = gameObject.GetComponent<LevelController>();
    }

    public RoomInternal catalog;

    private void Start()
    {
        controller.Catalog = catalog;
    }

    public List<EntityController> GetEntities()
    {
        return watcher.entities;
    }

    public void Update()
    {
        Vector3 avgPlayerPos = new Vector3(0f, 0f, 0f);
        List<GameObject> players = GameManager.Instance?.GameState?.getPlayers();
        if (players != null)
            if (players.Count > 0)
            {
                for (int i = 0; i < players.Count; i++)
                    avgPlayerPos += players[i].transform.position;
                avgPlayerPos /= players.Count;
            }

        float distance = Vector3.Distance(avgPlayerPos, controller.LastRoomPos);

        if (distance < (roomGenerationDistance))
        {
            controller.GenerateNewRoomConfig();
        }
        if (controller.RoomGeneratedCount != controller.RoomCount)
            controller.UpdateMap();
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(controller?.LastRoomPos ?? Vector3.zero, roomGenerationDistance);
    }
}
