using UnityEngine;
using System.Collections.Generic;
using System;


public class LevelController : MonoBehaviour
{
    public int seed;
    [SerializeField]
    float roomGenerationDistance = 0.5f;

    [SerializeField]
    private RoomInternal catalog;

    [SerializeField]
    List<RoomConfig> roomConfigs = new List<RoomConfig>();


    [SerializeField]
    List<RoomManager> roomsGenerated = new List<RoomManager>();


    private Vector3 lastRoomPosition = Vector2.zero;
    private GameObject map = null;

    void Start()
    {
        // deactivate initialRoom
        map = new GameObject("Map");
        GenerateNewRoomConfig();
    }
    private RoomConfig GetRandomRoom(RoomInternal catalog, RoomConfig previousRoom = null)
    {
        RoomConfig newRoomConfig = new RoomConfig();
        newRoomConfig.gridSize = 5;

        if (previousRoom != null)
        {
            Debug.Log("New Difficulty Level: " + previousRoom.difficultyLevel);
            newRoomConfig.difficultyLevel = previousRoom.difficultyLevel + 1;
            newRoomConfig.seed = previousRoom.seed + 42; // :D
            UnityEngine.Random.InitState(newRoomConfig.seed);

            // Set other random properties for the new room
            newRoomConfig.width =  UnityEngine.Random.Range(1, (previousRoom.difficultyLevel) + 1) * 2 - 1;
            newRoomConfig.height = UnityEngine.Random.Range(1, (previousRoom.difficultyLevel) + 1) * 2 - 1;
            newRoomConfig.offset = new Vector3(
                previousRoom.offset.x + previousRoom.width / 2f + newRoomConfig.width / 2f,
                0,
                previousRoom.offset.y
            );

            newRoomConfig.doorConfigs = new List<Door>();
            Door doorLeft = new Door();
            doorLeft.isConnected = false;
            doorLeft.wallType = Wall.WallType.Left;
            newRoomConfig.doorConfigs.Add(doorLeft);

            Door doorRight = new Door();
            doorRight.isConnected = false;
            doorRight.wallType = Wall.WallType.Right;
            newRoomConfig.doorConfigs.Add(doorRight);
        }
        else
        {
            newRoomConfig.difficultyLevel = 1;
            newRoomConfig.seed = 42;
            UnityEngine.Random.InitState(newRoomConfig.seed);
            newRoomConfig.roomType = RoomConfig.RoomType.Start;
            newRoomConfig.offset = new Vector3(0, 0, 0);
            newRoomConfig.width = 3;
            newRoomConfig.height = 3;

            newRoomConfig.doorConfigs = new List<Door>();
            Door initDoorRight = new Door();
            initDoorRight.isConnected = false;
            initDoorRight.wallType = Wall.WallType.Right;
            newRoomConfig.doorConfigs.Add(initDoorRight);
        }

        // Randomly select GameObjects from the lists
        newRoomConfig.wallTypes = new List<GameObject> { catalog.wallTypes[UnityEngine.Random.Range(0, catalog.wallTypes.Count)] };
        newRoomConfig.floorTypes = new List<GameObject> { catalog.floorTypes[UnityEngine.Random.Range(0, catalog.floorTypes.Count)] };
        newRoomConfig.singleDoorTypes = new List<GameObject> { catalog.singleDoorTypes[UnityEngine.Random.Range(0, catalog.singleDoorTypes.Count)] };
        newRoomConfig.doubleDoorTypes = new List<GameObject> { catalog.doubleDoorTypes[UnityEngine.Random.Range(0, catalog.doubleDoorTypes.Count)] };
        
        int interactableRandomIndex = UnityEngine.Random.Range(0, catalog.interactableTypes.Count);
        for (int i = 0; i < LevelScaler.GetNumberOfInteractablesToAdd(newRoomConfig.difficultyLevel); i++)
            newRoomConfig.interactableTypes = new List<GameObject> { catalog.interactableTypes[interactableRandomIndex] };
        
        int entityRandomIndex = UnityEngine.Random.Range(0, catalog.entityTypes.Count);
        for (int i = 0; i < LevelScaler.GetNumberOfEnemiesToAdd(newRoomConfig.difficultyLevel); i++)
            newRoomConfig.entityTypes = new List<GameObject> { catalog.entityTypes[entityRandomIndex] };

        return newRoomConfig;
    }



    private void ClearMap()
    {
        Destroy(map);
        foreach (RoomManager room in roomsGenerated)
            Destroy(room);


        lastRoomPosition = Vector3.zero;
        roomConfigs = new List<RoomConfig>();
        roomsGenerated = new List<RoomManager>();
        map = null;
    }

    private void GenerateMap()
    {
        for (int i = 0; i < roomConfigs.Count; i++)
        {
            GenerateRoom(roomConfigs[i]);
        }
    }

    private void UpdateMap()
    {

        if (roomsGenerated.Count < roomConfigs.Count)
        {
            GenerateRoom(roomConfigs[roomsGenerated.Count]);
            return;
        }

        if (roomsGenerated.Count > roomConfigs.Count)
        {
            Debug.Log("Regenerating map");
            Regenerate();
            return;
        }
    }

    private void Regenerate()
    {
        ClearMap();
        map = new GameObject("Map");
        GenerateNewRoomConfig();
    }
    void GenerateRoom(RoomConfig localRoomConfig)
    {
        Vector3 position = new Vector3(localRoomConfig.offset.x * localRoomConfig.gridSize, 0f, localRoomConfig.offset.y * localRoomConfig.gridSize);
        GameObject room = new GameObject("Room");
        room.transform.parent = map.transform;
        room.transform.position = position;

        room.name = localRoomConfig.roomType.ToString() + " (" + localRoomConfig.offset.x + ", " + localRoomConfig.offset.y + ")";
        room.SetActive(true);
        room.transform.parent = map.transform;
        room.name = localRoomConfig.roomType.ToString() + " (" + localRoomConfig.offset.x + ", " + localRoomConfig.offset.y + ")";
        room.gameObject.AddComponent<RoomManager>();
        RoomManager roomManager = room.GetComponent<RoomManager>();
        roomManager.UpdateConfig(localRoomConfig);
        if (roomsGenerated.Count != 0)
        {
            // connect doors
            // inside RoomManager's game object are DoorController , get all door controllers of this room and the previous room
            DoorController[] doorControllers = roomManager.gameObject.GetComponentsInChildren<DoorController>();
            DoorController[] previousDoorControllers = roomsGenerated[roomsGenerated.Count - 1].GetComponentsInChildren<DoorController>();


            // now check for the right door of previous room and connect it with the left door of this room
            for (int i = 0; i < doorControllers.Length; i++)
                for (int j = 0; j < previousDoorControllers.Length; j++)
                {
                    DoorController doorController = doorControllers[i];
                    DoorController previousDoorController = previousDoorControllers[j];

                    doorController.AssignDoor(previousDoorController);
                    previousDoorController.AssignDoor(doorController);

                    roomManager.watcher.doors.Add(doorController);
                    roomsGenerated[roomsGenerated.Count - 1].watcher.doors.Add(previousDoorController);
                }
        }
        roomsGenerated.Add(roomManager);
        lastRoomPosition = new Vector3(localRoomConfig.offset.x * localRoomConfig.gridSize, 0f, localRoomConfig.offset.y * localRoomConfig.gridSize);
    }

    private void Update()
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

        float distance = Vector3.Distance(avgPlayerPos, lastRoomPosition);

        if (distance < (roomGenerationDistance))
        {
            GenerateNewRoomConfig();
        }
        if (roomsGenerated.Count != roomConfigs.Count)
            UpdateMap();
    }

    private void GenerateNewRoomConfig()
    {
        RoomConfig currentRoom;
        if (roomConfigs.Count == 0)
        {
            roomConfigs.Add(GetRandomRoom(catalog));

            return;
        }
        else
        {
            currentRoom = roomConfigs[roomConfigs.Count - 1];
        }

        List<Door> unconnectedDoors = GetUnconnectedDoors(currentRoom);
        if (unconnectedDoors.Count == 0)
        {
            Debug.Log("No unconnected doors");
            return;
        }

        RoomConfig newRoomConfig = GetRandomRoom(catalog, currentRoom);
        roomConfigs.Add(newRoomConfig);
        currentRoom.Connect(newRoomConfig.doorConfigs[0]); // Assuming we are connecting the first door of the new room to the current room.
    }


    private List<Door> GetUnconnectedDoors(RoomConfig roomConfig)
    {
        List<Door> unconnectedDoors = new List<Door>();
        foreach (Door door in roomConfig.doorConfigs)
            if (!door.isConnected)
                unconnectedDoors.Add(door);
        return unconnectedDoors;
    }

    void OnDrawGizmos()
    {
        foreach (RoomConfig roomConfig in roomConfigs)
        {
            // Vector3 pos = roomConfig.transform.position;
            Vector3 pos = new Vector3(roomConfig.offset.x * roomConfig.gridSize, 0, roomConfig.offset.y * roomConfig.gridSize);
            Vector3 scale = new Vector3(roomConfig.width, 1f, roomConfig.height) * roomConfig.gridSize;
            Gizmos.DrawWireCube(pos, scale);
            Gizmos.color = Color.green;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(lastRoomPosition, roomGenerationDistance);
    }


    void OnValidate()
    {
        roomConfigs = new List<RoomConfig>();
    }

}
