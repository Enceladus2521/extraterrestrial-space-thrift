using UnityEngine;
using System.Collections.Generic;

public class LevelController : MonoBehaviour
{
    private RoomInternal catalog;

    [SerializeField] private List<RoomConfig> roomConfigs = new List<RoomConfig>();
    [SerializeField] private List<RoomManager> roomsGenerated = new List<RoomManager>();
    private Vector3 lastRoomPosition = Vector2.zero;
    private GameObject map = null;

    public RoomInternal Catalog { set { catalog = value; } }
    public Vector3 LastRoomPos { get { return lastRoomPosition; } }
    public int RoomCount { get { return roomConfigs.Count; } }
    public int RoomGeneratedCount { get { return roomsGenerated.Count; } }
    public List<RoomManager> RoomsGenerated { get { return roomsGenerated; } }
    
    void Start()
    {
        map = new GameObject("Map");
        roomConfigs.Add(GetRandomRoom(catalog));
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
            Random.InitState(newRoomConfig.seed);

            newRoomConfig.width = Random.Range(1, (previousRoom.difficultyLevel) + 1);
            if (previousRoom.width == 2)
                newRoomConfig.width += 1;
            newRoomConfig.height = Random.Range(1, (previousRoom.difficultyLevel) + 1);
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
            newRoomConfig.seed = GameManager.Instance.Seed;
            Random.InitState(newRoomConfig.seed);
            newRoomConfig.roomType = RoomConfig.RoomType.Start;
            newRoomConfig.offset = Vector3.zero;
            newRoomConfig.width = 3;
            newRoomConfig.height = 3;

            newRoomConfig.doorConfigs = new List<Door>();
            Door initDoorRight = new Door();
            initDoorRight.isConnected = false;
            initDoorRight.wallType = Wall.WallType.Right;
            newRoomConfig.doorConfigs.Add(initDoorRight);
        }

        newRoomConfig.wallTypes = new List<GameObject> { catalog.wallTypes[Random.Range(0, catalog.wallTypes.Count)] };
        newRoomConfig.floorTypes = new List<GameObject> { catalog.floorTypes[Random.Range(0, catalog.floorTypes.Count)] };
        newRoomConfig.singleDoorTypes = new List<GameObject> { catalog.singleDoorTypes[Random.Range(0, catalog.singleDoorTypes.Count)] };
        newRoomConfig.doubleDoorTypes = new List<GameObject> { catalog.doubleDoorTypes[Random.Range(0, catalog.doubleDoorTypes.Count)] };

        int interactableRandomIndex = Random.Range(0, catalog.interactableTypes.Count);
        newRoomConfig.interactableTypes = new List<GameObject> { catalog.interactableTypes[interactableRandomIndex] };

        int entityRandomIndex = Random.Range(0, catalog.entityTypes.Count);
        newRoomConfig.entityTypes = new List<GameObject> { catalog.entityTypes[entityRandomIndex] };

        return newRoomConfig;
    }

    public void GenerateNewRoomConfig()
    {
        RoomConfig newRoomConfig = GetRandomRoom(catalog, roomConfigs.Count > 0 ? roomConfigs[roomConfigs.Count - 1] : null);
        roomConfigs.Add(newRoomConfig);
        // GenerateNewRoom(newRoomConfig);
    }

    public void GenerateNewRoom()
    {
        RoomConfig newRoomConfig = roomConfigs[roomsGenerated.Count];
        GameObject newRoom = new GameObject($"Room_{roomsGenerated.Count}");
        newRoom.transform.parent = map.transform;
        newRoom.transform.position = newRoomConfig.offset * newRoomConfig.gridSize;

        RoomManager roomManager = newRoom.AddComponent<RoomManager>();
        roomManager.InitRoom(newRoomConfig);
        roomsGenerated.Add(roomManager);

        if (roomsGenerated.Count > 1)
        {
            RoomManager previousRoomManager = roomsGenerated[roomsGenerated.Count - 2];
            ConnectDoors(previousRoomManager, roomManager);
        }

        lastRoomPosition = newRoom.transform.position;
    }
    private void ConnectDoors(RoomManager roomA, RoomManager roomB)
    {
        // inside RoomManager's game object are DoorController , get all door controllers of this room and the previous room
        DoorController[] doorControllers = roomA.gameObject.GetComponentsInChildren<DoorController>();
        DoorController[] previousDoorControllers = roomB.GetComponentsInChildren<DoorController>();


        // now check for the right door of previous room and connect it with the left door of this room
        for (int i = 0; i < doorControllers.Length; i++)
            for (int j = 0; j < previousDoorControllers.Length; j++)
            {
                DoorController doorController = doorControllers[i];
                DoorController previousDoorController = previousDoorControllers[j];

                doorController.AssignDoor(previousDoorController);
                previousDoorController.AssignDoor(doorController);

                roomA.watcher.doors.Add(doorController);
                roomB.watcher.doors.Add(previousDoorController);
            }
    }


    public void Clear()
    {
        // Remove all previously generated rooms and clear the lists
        foreach (var room in roomsGenerated)
        {
            Destroy(room.gameObject);
        }
        roomsGenerated.Clear();
        roomConfigs.Clear();
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

    }
}
