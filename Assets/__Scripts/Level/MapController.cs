using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MapConfig
{
    public int maxHeight;
    public int maxWidth;
    public int seed;
}

public class MapController : MonoBehaviour
{
    [SerializeField]
    RoomController initialRoom = null;
    [SerializeField]
    public MapConfig mapConfig;

    [SerializeField]
    List<Room> roomConfigs = new List<Room>();

    [SerializeField]
    List<RoomController> roomsGenerated = new List<RoomController>();

    [SerializeField]
    public float roomGenerationDistance = 0.5f;

    private Vector3 lastRoomPosition = Vector2.zero;
    private GameObject map = null;

    void Start()
    {
        map = new GameObject("Map");
        Random.InitState(mapConfig.seed);
        UnityEngine.Random.InitState(mapConfig.seed);
        GenerateNewRoomConfig();
    }

    private void ClearMap()
    {
        Debug.Log("ClearMap");
        roomConfigs = new List<Room>();
        lastRoomPosition = Vector3.zero;
        // delete all roomsGenerated gameobjects
        foreach (RoomController room in roomsGenerated)
        {
            DestroyImmediate(room.gameObject);
        }
        roomsGenerated = new List<RoomController>();
        DestroyImmediate(map);
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
            Debug.LogWarning("Generated Room: " + roomsGenerated.Count + " / " + roomConfigs.Count);
            ClearMap();
            GenerateRoom(roomConfigs[0]);
            return;
        }
    }

    void GenerateRoom(Room localRoomConfig)
    {
        GameObject room = Instantiate(localRoomConfig.transform.gameObject);
        if (room == null) return;
        if (map == null) return;
        room.transform.parent = map.transform;
        room.name = localRoomConfig.roomType.ToString() + " (" + localRoomConfig.offset.x + ", " + localRoomConfig.offset.y + ")";
        RoomController roomController = room.GetComponent<RoomController>();
        roomsGenerated.Add(roomController);
        roomController.UpdateConfig(localRoomConfig);
        roomController.Generate();
        lastRoomPosition = new Vector3(localRoomConfig.offset.x * localRoomConfig.gridSize, 0f, localRoomConfig.offset.y * localRoomConfig.gridSize);
    }

    private void Update()
    {
        Vector3 avgPlayerPos = new Vector3(0f, 0f, 0f);
        List<GameObject> players = GameManager.Instance?.GameState?.getPlayers();
        if (players != null)
            if (players.Count > 0){
                for (int i = 0; i < players.Count; i++)
                {
                    avgPlayerPos += players[i].transform.position;
                } 
                avgPlayerPos /= players.Count;
            }

        float distance = Vector3.Distance(avgPlayerPos, lastRoomPosition);

        if (distance < (roomGenerationDistance ))
        {
            GenerateNewRoomConfig();
        }
        if (roomsGenerated.Count != roomConfigs.Count) UpdateMap();
    }


    private void GenerateNewRoomConfig()
    {   
        Room currentRoom;
        if (roomConfigs.Count == 0) {
            currentRoom = initialRoom.roomConfig;
            foreach (Door door in currentRoom.doorConfigs)
            {
                door.isConnected = false;
            }
            roomConfigs.Add(currentRoom);
        }else{
            currentRoom = roomConfigs[roomConfigs.Count - 1];
        }

        List<Door> unconnectedDoors = GetUnconnectedDoors(currentRoom);
        if (unconnectedDoors.Count == 0)
        {
            Debug.Log("No Unconnected Doors");
            return;
        }

        Room newRoomConfig = new Room();

        newRoomConfig.width =  Random.Range(1, mapConfig.maxWidth + 1);
        newRoomConfig.height = Random.Range(1, mapConfig.maxHeight + 1);
        newRoomConfig.offset = new Vector3(
            currentRoom.offset.x + currentRoom.width / 2f + newRoomConfig.width / 2f,
            0,
            currentRoom.offset.y
        );
        newRoomConfig.roomType = Room.RoomType.Basic;
        newRoomConfig.internalConfig = currentRoom.internalConfig;
        newRoomConfig.transform = currentRoom.transform;
        newRoomConfig.gridSize = currentRoom.gridSize;
        newRoomConfig.seed = currentRoom.seed + (roomConfigs.Count + 1);

        Door doorLeft = new Door();
        doorLeft.isConnected = false;
        doorLeft.wallType = Wall.WallType.Left;
        newRoomConfig.doorConfigs.Add(doorLeft);

        Door doorRight = new Door();
        doorRight.isConnected = false;
        doorRight.wallType = Wall.WallType.Right;
        newRoomConfig.doorConfigs.Add(doorRight);


        Room roomLeft = roomConfigs[roomConfigs.Count - 1];
        roomConfigs.Add(newRoomConfig);
        roomLeft.Connect(doorLeft);
    }

    private List<Door> GetUnconnectedDoors(Room roomConfig)
    {
        List<Door> unconnectedDoors = new List<Door>();
        foreach (Door door in roomConfig.doorConfigs)
        {
            if (!door.isConnected)
                unconnectedDoors.Add(door);
        }
        return unconnectedDoors;
    }

    private Room GetRandomRoomConfig()
    {
        return roomConfigs[UnityEngine.Random.Range(0, roomConfigs.Count)];
    }

    void OnDrawGizmos()
    {
        foreach (Room roomConfig in roomConfigs)
        {
            Vector3 pos = roomConfig.transform.position;
            pos += new Vector3(roomConfig.offset.x * roomConfig.gridSize, 0, roomConfig.offset.y * roomConfig.gridSize);
            Vector3 scale = new Vector3(roomConfig.width, 1f, roomConfig.height) * roomConfig.gridSize;
            Gizmos.DrawWireCube(pos, scale);
            Gizmos.color = Color.green;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(lastRoomPosition, roomGenerationDistance);
    }


    void OnValidate()
    {
        roomConfigs = new List<Room>();
    }

}
