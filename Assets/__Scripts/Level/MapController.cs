using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MapConfig
{
    public int maxHeight;
    public int maxWidth;
    public int seed;
    [Range(1, 100)]
    public int rooms;
}

public class MapController : MonoBehaviour
{
    [SerializeField]
    RoomController initialRoom = null;
    [SerializeField]
    public MapConfig mapConfig;

    [SerializeField]
    List<RoomConfig> roomConfigs = new List<RoomConfig>();


    void Start()
    {
        UnityEngine.Random.InitState(mapConfig.seed);
        GenerateRoomConfigs();
        GenerateMap();
    }


    private void GenerateRoomConfigs()
    {
        RoomConfig currentRoom = initialRoom.roomConfig(    );
        roomConfigs.Clear();
        roomConfigs.Add(currentRoom);
        GenerateRoomsRecursive(currentRoom);
    }

    private void GenerateMap()
    {
        GameObject map = new GameObject("Map");

        // Generate Childrens for Initial Room
        for (int i = 0; i < mapConfig.rooms; i++)
        {
            RoomConfig localRoomConfig = roomConfigs[i];
            GameObject room = Instantiate(localRoomConfig.transform.gameObject);
            room.transform.parent = map.transform;
            room.name = localRoomConfig.roomType.ToString() + " (" + localRoomConfig.offset.x + ", " + localRoomConfig.offset.y + ")";       
            // room.AddComponent<RoomController>();

            RoomController roomController = room.GetComponent<RoomController>();
            roomController.UpdateConfig(localRoomConfig);
            roomController.Generate();

            // roomConfigs.Add(RoomController.roomConfig());
        
                // Instantiate(roomConfigs[i].transform);
        }
    }

    private void GenerateRoomsRecursive(RoomConfig currentRoom)
    {

        float spacingBetweenRooms = 0; 

        if (roomConfigs.Count >= mapConfig.rooms)
            return;
        List<DoorConfig> unconnectedDoors = GetUnconnectedDoors(currentRoom);
        if (unconnectedDoors.Count == 0)
            return;
        int targetRooms = mapConfig.rooms - roomConfigs.Count;
        int roomsToGenerate = Mathf.Min(targetRooms, unconnectedDoors.Count);

        for (int i = 0; i < roomsToGenerate; i++)
        {
            RoomConfig newRoomConfig = new RoomConfig();
            newRoomConfig.width = Random.Range(1, mapConfig.maxWidth + 1);
            newRoomConfig.height = Random.Range(1, mapConfig.maxHeight + 1);
            newRoomConfig.offset = new Vector3(
                currentRoom.offset.x + currentRoom.width / 2f + newRoomConfig.width / 2f, 
                0, 
                currentRoom.offset.y
            );
            newRoomConfig.roomType = RoomController.RoomType.Basic;
            newRoomConfig.transform = currentRoom.transform;
            newRoomConfig.gridSize = currentRoom.gridSize;
            newRoomConfig.seed = currentRoom.seed + i;
            roomConfigs.Add(newRoomConfig);

            DoorConfig doorLeft = new DoorConfig();
            doorLeft.isConnected = true;
            newRoomConfig.doorConfigs.Add(doorLeft);

            DoorConfig doorRight = new DoorConfig();
            doorRight.isConnected = true;
            newRoomConfig.doorConfigs.Add(doorRight);

            currentRoom = newRoomConfig;

            GenerateRoomsRecursive(newRoomConfig);
        }


    }

    private List<DoorConfig> GetUnconnectedDoors(RoomConfig roomConfig)
    {
        List<DoorConfig> unconnectedDoors = new List<DoorConfig>();
        foreach (DoorConfig door in roomConfig.doorConfigs)
        {
            if (!door.isConnected)
                unconnectedDoors.Add(door);
        }
        return unconnectedDoors;
    }

    private RoomConfig GetRandomRoomConfig()
    {
        return roomConfigs[UnityEngine.Random.Range(0, roomConfigs.Count)];
    }

    void OnDrawGizmos()
    {
        foreach (RoomConfig roomConfig in roomConfigs)
        {
            Vector3 pos = roomConfig.transform.position;
            pos += new Vector3(roomConfig.offset.x * roomConfig.gridSize, 0, roomConfig.offset.y * roomConfig.gridSize);
            Vector3 scale = new Vector3(roomConfig.width, 1f,  roomConfig.height) * roomConfig.gridSize;
            Gizmos.DrawWireCube(pos, scale);
            Gizmos.color = Color.green;

        }
    }


    void OnValidate()
    {
        GenerateRoomConfigs();
    }

}
