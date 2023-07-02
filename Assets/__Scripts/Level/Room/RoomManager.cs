using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    private AudioClip[] roomFinishedSounds; // Remove the old serialized field

    private AudioSource audioSource;


    public RoomConfig RoomConfig { get { return roomConfig; } }
    RoomController controller;
    [SerializeField]
    public Watcher watcher;
    Watcher voidWatcher;

    [SerializeField]
    bool isActive = false;
    [SerializeField]
    bool isLocked = true;

    public int Difficulty { get { return roomConfig.difficulty; } }
    public bool IsLocked { get { return isLocked; } }

    void Awake()
    {
        watcher = new Watcher();
        voidWatcher = new Watcher();

        if (gameObject.GetComponent<AudioSource>() == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        else
            audioSource = GetComponent<AudioSource>();

        // Load all the audio clips from "Resources/Audio/Event/Cleared" folder
        roomFinishedSounds = Resources.LoadAll<AudioClip>("Audio/Room/Cleared");
        AudioMixer mainMixer = Resources.Load<AudioMixer>("Audio/Mixer/Main");
        if (mainMixer == null)
        {
            Debug.LogError("Could not load main mixer");
            return;
        }

        // Get the "Music" group from the AudioMixer
        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups("Ambiance");
        if (groups.Length > 0)
        {
            audioSource.outputAudioMixerGroup = groups[0];
        }
        else
        {
            Debug.LogError("Could not find Music group in the main mixer");
        }

    }
    private void CreateDeathZone()
    {
        // Calculate the size of the box collider based on the room configuration
        float deathZoneWidth = roomConfig.width * roomConfig.gridSize * 10f;
        float deathZoneHeight = roomConfig.height * roomConfig.gridSize * 10f;
        float deathZoneDepth = 20f; // The thickness of the death zone collider

        // Calculate the position of the center of the death zone collider
        Vector3 deathZoneCenter = Vector3.zero;
        deathZoneCenter.y -= (roomConfig.gridSize * 1f) + (deathZoneDepth / 2f);

        // Create the death zone box collider
        BoxCollider deathZoneCollider = gameObject.AddComponent<BoxCollider>();
        deathZoneCollider.size = new Vector3(deathZoneWidth, deathZoneDepth, deathZoneHeight);
        deathZoneCollider.center = deathZoneCenter;

        // Add a trigger to the collider so that it doesn't physically interact with objects
        deathZoneCollider.isTrigger = true;
    }

    // on trigger enter, create the death zone
    private void OnTriggerEnter(Collider other)
    {
        // if is player than take damge infinity
        if (other.tag == "Player")
        {
            Debug.Log("Player");
            other.GetComponent<PlayerStats>().TakeDamage(Mathf.Infinity);
        }
        else if (other.tag == "Enemy")
        {
            Debug.Log("Enemy");
            other.GetComponent<EnemyController>().TakeDamage(Mathf.Infinity);
        }
        else
        {
            Debug.Log("Other");
            Destroy(other.gameObject);
        }
    }

    void Start()
    {
        // Call the function to create the death zone collider on initialization
        CreateDeathZone();
    }

    RoomConfig lastRoomConfig;
    public void InitRoom(RoomConfig config)
    {

        if (lastRoomConfig == roomConfig && lastRoomConfig != null) return;

        lastRoomConfig = roomConfig;
        roomConfig = config;
        if (gameObject.GetComponent<RoomController>() == null)
            controller = gameObject.AddComponent<RoomController>();
        else
            controller = gameObject.GetComponent<RoomController>();
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
        bool isInsideWidth = position.y > roomPosition.z - roomHeight / 2 && position.y < roomPosition.z + roomHeight / 2;
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
        List<EnemyController> entities = LevelManager.Instance?.Entities;
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
            }
            else
            {
                watcher.entities.Remove(entity);
                voidWatcher.entities.Add(entity);
            }
        }

        // if enemies are 0 unlock room
        if (GetEnemys().Count == 0 && isActive)
        {
            if (isLocked)
            {
                GameManager.Instance?.OnRoomFinished(this);

                if (roomConfig.difficulty != 1)
                    if (roomFinishedSounds.Length > 0)
                    {
                        int randomIndex = Random.Range(0, roomFinishedSounds.Length);
                        audioSource.PlayOneShot(roomFinishedSounds[randomIndex]);

                    }

            }
            isLocked = false;
        }
        else
            isLocked = true;
    }
    private void UpdatePlayers()
    {
        // TODO: get players not by tag
        List<GameObject> playersGlobal = GameManager.Instance?.Players;
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

    // on trigger

    public Vector3 GetRoomCenter()
    {
        return transform.position;
    }
}