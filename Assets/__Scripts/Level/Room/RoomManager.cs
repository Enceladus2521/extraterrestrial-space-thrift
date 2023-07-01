using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Watcher
{
    public List<DoorController> doors = new List<DoorController>();
    public List<Interacter> interactables = new List<Interacter>();
    public List<EnemyController> entities = new List<EnemyController>();

    public List<PlayerStats> players = new List<PlayerStats>();
}

public class RoomManager : MonoBehaviour
{
    // constructor for Room Watcher is dead true
    private RoomConfig roomConfig;
    public RoomConfig RoomConfig { get { return roomConfig; } }
    RoomController controller;
    [SerializeField]
    public Watcher watcher;
    Watcher voidWatcher;

    [SerializeField]
    bool isActive = false;
    [SerializeField]
    bool isLocked = true;

    void Awake()
    {
        watcher = new Watcher();
        voidWatcher = new Watcher();
        if (gameObject.GetComponent<RoomController>() == null)
            controller = gameObject.AddComponent<RoomController>();
        else
            controller = gameObject.GetComponent<RoomController>();
    }
    RoomConfig lastRoomConfig;
    public void InitRoom(RoomConfig config)
    {

        if (lastRoomConfig == roomConfig && lastRoomConfig != null) return;

        lastRoomConfig = roomConfig;
        roomConfig = config;
        controller.UpdateConfig(config);

    }

    List<EnemyController> GetEnemys()
    {
        List<EnemyController> enemys = new List<EnemyController>();
        for (int i = 0; i < watcher.entities.Count; i++)
        {
            enemys.Add(watcher.entities[i]);
            // TODO: Future: We could have passive entities here
        }
        return enemys;
    }

    bool IsPositionInRoom(Vector2 position)
    {
        float roomWidth = roomConfig.width * roomConfig.gridSize;
        float roomHeight = roomConfig.height * roomConfig.gridSize;
        Vector3 roomPosition = new Vector3(roomConfig.offset.x * roomConfig.gridSize, 0, roomConfig.offset.y * roomConfig.gridSize);
        bool isInsideHeight = position.x > roomPosition.x - roomWidth / 2 && position.x < roomPosition.x + roomWidth / 2;
        bool isInsideWidth = position.y > roomPosition.y - roomHeight / 2 && position.y < roomPosition.y + roomHeight / 2;
        return isInsideHeight && isInsideWidth;
    }

    public void Update()
    {
        UpdatePlayers();


        if (!isActive) return;
    
        UpdateEntities();
        UpdateInteractables();
        UpdateDoors();
    }

    void OnValidate()
    {
        watcher = new Watcher();
        voidWatcher = new Watcher();
    }


    private void UpdateInteractables()
    {
        for (int i = 0; i < watcher.interactables.Count; i++)
        {
            Interacter interactable = watcher.interactables[i];
            if (interactable.gameObject != null)
                watcher.interactables.Remove(interactable);
            else
                watcher.interactables.Add(interactable);
        }
    }
    private void UpdateEntities()
    {
        // loop over all entities and check in entity state for is alive in health state#
        watcher?.entities.Clear();
        List<EnemyController> entities = LevelManager.Instance?.GetEntities();
        for (int i = 0; i < entities.Count; i++)
        {
            EnemyController entity = entities[i];
            if (entity == null) continue;
            // EntityController entity = entities[i].GetComponent<EntityController>();
            if (IsPositionInRoom(entity.transform.position) && entity.gameObject.activeInHierarchy)
            {
                watcher.entities.Add(entity);
                voidWatcher.entities.Remove(entity);
                // bool isAlive = entity.state.healthStats.IsAlive;
                // // if is not alive move to void watcher
                // if (!isAlive)
                // {
                //     voidWatcher.entities.Add(entity);
                //     // and remove from list
                //     watcher.entities.Remove(entity);
                // }
            }else{
                watcher.entities.Remove(entity);
                voidWatcher.entities.Add(entity);
            }
        }

        // if enemies are 0 unlock room
        if (GetEnemys().Count == 0 && isActive)
            isLocked = false;
        else
            isLocked = true;
    }
    private void UpdatePlayers()
    {
        // TODO: get players not by tag
        List<GameObject> playersGlobal = GameManager.Instance?.GameState?.getPlayers();
        watcher.players.Clear();

        if (playersGlobal != null)
            foreach (GameObject player in playersGlobal)
            {
                if (player != null)
                {
                    PlayerStats playerStats = player.GetComponent<PlayerStats>();
                    if (playerStats != null)
                    {
                        if (IsPositionInRoom(playerStats.transform.position))
                        {
                            watcher.players.Add(playerStats);
                        }
                    }
                }
            }

        if (watcher.players.Count == 0)
            isActive = false;
        else
            isActive = true;
    }

    private void UpdateDoors()
    {
        for (int i = 0; i < watcher?.doors.Count; i++)
        {
            DoorController door = watcher.doors[i];
            door.ToggleDoorLockState(isLocked);
        }
    }

    void OnDrawGizmos()
    {
        // if player count > 0 than render a green filled sphere -5 to under the room and if its 0 render a red filled sphere 5 to under the room
        if (isActive)
            if (isLocked)
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Vector3 pos = transform.position;
        pos.y -= 42;

        Gizmos.DrawSphere(pos, 5);
    }
}