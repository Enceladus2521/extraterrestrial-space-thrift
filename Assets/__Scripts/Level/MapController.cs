using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class MapConfig
{
    public int seed;
    public int rooms;
    public RoomGenerator initialRoom;
}

public class MapController : MonoBehaviour
{
    public MapConfig mapConfig;
    public LevelGenerator levelGenerator;

    void Start()
    {
        Random.InitState(mapConfig.seed);
        GenerateMap(mapConfig.rooms);
    }

    void GenerateMap(int numRooms)
    {
        List<RoomConfig> roomConfigs = new List<RoomConfig>();

        // Create the initial room
        RoomConfig initialRoom = new RoomConfig
        {
            roomType = mapConfig.initialRoom.roomType,
            offset = Vector2Int.zero,
            hasDoorUp = mapConfig.initialRoom.hasDoorUp,
            hasDoorDown = mapConfig.initialRoom.hasDoorDown,
            hasDoorRight = mapConfig.initialRoom.hasDoorRight,
            hasDoorLeft = mapConfig.initialRoom.hasDoorLeft,
            width = mapConfig.initialRoom.width,
            height = mapConfig.initialRoom.height
        };
        roomConfigs.Add(initialRoom);

        // Create new rooms
        for (int i = 1; i < numRooms; i++)
        {
            RoomConfig previousRoom = roomConfigs[i - 1];
            RoomConfig newRoom = CreateNewRoom(previousRoom, i);
            roomConfigs.Add(newRoom);
        }

        levelGenerator.GenerateRooms(configs: roomConfigs);
    }

    RoomConfig CreateNewRoom(RoomConfig previousRoom, int roomIndex)
    {
        Random.InitState(mapConfig.seed + roomIndex); // Set the new seed for each room

        Vector2Int direction = GetRandomDirection(previousRoom);
        Vector2Int offset = previousRoom.offset + direction;

        RoomConfig newRoom = new RoomConfig
        {
            roomType = RoomGenerator.RoomType.Basic,
            offset = offset,
            hasDoorUp = direction == Vector2Int.up,
            hasDoorDown = direction == Vector2Int.down,
            hasDoorRight = direction == Vector2Int.right,
            hasDoorLeft = direction == Vector2Int.left,
            width = mapConfig.initialRoom.width,
            height = mapConfig.initialRoom.height
        };
        return newRoom;
    }

    Vector2Int GetRandomDirection(RoomConfig room)
    {
        List<Vector2Int> availableDirections = new List<Vector2Int>();

        if (room.hasDoorUp)
            availableDirections.Add(Vector2Int.up);
        if (room.hasDoorDown)
            availableDirections.Add(Vector2Int.down);
        if (room.hasDoorRight)
            availableDirections.Add(Vector2Int.right);
        if (room.hasDoorLeft)
            availableDirections.Add(Vector2Int.left);

        return availableDirections[Random.Range(0, availableDirections.Count)];
    }

    IEnumerator GenerateMapCoroutine()
    {
        yield return null;
        GenerateMap(mapConfig.rooms);
    }

    void OnValidate()
    {
        StartCoroutine(GenerateMapCoroutine());
    }
}
