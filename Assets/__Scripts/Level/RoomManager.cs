using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomManager
{
    // constructor for Room Watcher is dead true
    RoomConfig roomConfig;


    public RoomManager(RoomConfig roomConfig)
    {
        this.roomConfig = roomConfig;
        this.Update(roomConfig);
    }

    public RoomManager(bool isDead)
    {
        this.isDead = isDead;
        if (!isDead)
        {
            voidWatcher = new RoomManager(true);
        }
    }

    bool isDead = false;

    // List<GameObject> walls = new List<GameObject>();
    // List<GameObject> floors = new List<GameObject>();
    public List<DoorController> doors = new List<DoorController>();
    public List<Interacter> interactables = new List<Interacter>();
    public List<EntityController> entities = new List<EntityController>();
    List<EntityController> GetEnemys()
    {
        List<EntityController> enemys = new List<EntityController>();
        for (int i = 0; i < entities.Count; i++)
        {
            enemys.Add(entities[i]);
            // TODO: Future: We could have passive entities here
        }
        return enemys;
    }

    public List<PlayerMovementController> players = new List<PlayerMovementController>();

    // void room watcher
    public RoomManager voidWatcher;

    // Update existence of all objects
    public void Update(RoomConfig roomConfig)
    {
        this.roomConfig = roomConfig;
        UpdateEntities();
        UpdateInteractables();
        UpdateDoors();
        UpdatePlayers();
    }

    private void UpdateInteractables()
    {
        foreach (Interacter interactable in interactables)
        {
            if (interactable.gameObject != null)
                interactables.Remove(interactable);
            else
                interactables.Add(interactable);
        }
    }
    private void UpdateEntities()
    {
        // loop over all entities and check in entity state for is alive in health state
        foreach (EntityController entity in entities)
        {
            bool isAlive = entity.state.healthStats.IsAlive;
            // if is not alive move to void watcher
            if (!isAlive)
            {
                voidWatcher.entities.Add(entity);
                // and remove from list
                entities.Remove(entity);
            }
            else
            {
                // if is alive move to void watcher
                voidWatcher.entities.Add(entity);
            }
        }
    }
    private void UpdatePlayers()
    {
        List<GameObject> playersGlobal = GameManager.Instance?.GameState?.getPlayers();
        if (playersGlobal != null)
            foreach (GameObject player in playersGlobal)
            {
                if (player != null)
                {
                    PlayerMovementController playerController = player.GetComponent<PlayerMovementController>();
                    if (playerController != null)
                        players.Add(playerController);
                }
            }
    }

    private void UpdateDoors()
    {
        foreach (DoorController door in doors)
        {
            int enemysAlive = GetEnemys().Count;
            if (enemysAlive == 0)
            {
                door.isLocked = false;
            }
            else
            {
                door.isLocked = true;
            }
        }
    }

}