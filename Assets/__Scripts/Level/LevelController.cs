using UnityEngine;
using System.Collections.Generic;

public class LevelController : MonoBehaviour
{
    [SerializeField] private float roomGenerationDistance = 0.5f;
    [SerializeField] public RoomInternal catalog;
    [SerializeField] private List<RoomConfig> roomConfigs = new List<RoomConfig>();
    [SerializeField] private List<RoomManager> roomsGenerated = new List<RoomManager>();
    private Vector3 lastRoomPosition = Vector2.zero;
    private GameObject map = null;

    void Start()
    {
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
            Random.InitState(newRoomConfig.seed);

            newRoomConfig.width = Random.Range(1, (previousRoom.difficultyLevel) + 1) * 2 - 1;
            newRoomConfig.height = Random.Range(1, (previousRoom.difficultyLevel) + 1) * 2 - 1;
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

   private void GenerateNewRoomConfig()
    {
        RoomConfig newRoomConfig = GetRandomRoom(catalog, roomConfigs.Count > 0 ? roomConfigs[roomConfigs.Count - 1] : null);
        roomConfigs.Add(newRoomConfig);
        GenerateNewRoom(newRoomConfig);
    }

    private void GenerateNewRoom(RoomConfig newRoomConfig)
    {
        GameObject newRoom = new GameObject($"Room_{roomsGenerated.Count}");
        newRoom.transform.parent = map.transform;
        newRoom.transform.position = newRoomConfig.offset;

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
        List<Door> doorsA = roomA.rooms[0].doors;
        List<Door> doorsB = roomB.rooms[0].doors;

        Door doorA = doorsA[Random.Range(0, doorsA.Count)];
        Door doorB = doorsB[Random.Range(0, doorsB.Count)];

        roomA.ConnectDoors(doorA, doorB, roomB);
    }
}
