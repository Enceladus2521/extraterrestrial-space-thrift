using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MapConfig
{
    public int maxHeight;
    public int maxWidth;
    public int seed;
}

public class LevelController : MonoBehaviour
{
    [SerializeField]
    float roomGenerationDistance = 0.5f;

    [SerializeField]
    RoomController initialRoom = null;

    [SerializeField]
    public MapConfig mapConfig;

    [SerializeField]
    List<RoomConfig> roomConfigs = new List<RoomConfig>();


    [SerializeField]
    List<RoomController> roomsGenerated = new List<RoomController>();


    private Vector3 lastRoomPosition = Vector2.zero;
    private GameObject map = null;

    void Start()
    {
        // deactivate initialRoom
        map = new GameObject("Map");
        GenerateNewRoomConfig();
    }

    private void ClearMap()
    {
        Destroy(map);
        foreach (RoomController room in roomsGenerated)
            Destroy(room.gameObject);


        lastRoomPosition = Vector3.zero;
        roomConfigs = new List<RoomConfig>();
        roomsGenerated = new List<RoomController>();
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
        Random.InitState(mapConfig.seed);
        UnityEngine.Random.InitState(mapConfig.seed);
        GenerateNewRoomConfig();
    }
    void GenerateRoom(RoomConfig localRoomConfig)
    {
        Vector3 position = new Vector3(localRoomConfig.offset.x * localRoomConfig.gridSize, 0f, localRoomConfig.offset.y * localRoomConfig.gridSize);
        GameObject room = Instantiate(initialRoom.gameObject, position, Quaternion.identity);
        initialRoom.gameObject.SetActive(true);
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
            if (players.Count > 0)
            {
                for (int i = 0; i < players.Count; i++)
                    avgPlayerPos += players[i].transform.position;
                avgPlayerPos /= players.Count;
            }

        float distance = Vector3.Distance(avgPlayerPos, lastRoomPosition);

        if (distance < (roomGenerationDistance))
        {
            Debug.Log("Room Generation");
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
            currentRoom = initialRoom.roomConfig;
            // create a copy
            RoomConfig initRoomConfig = new RoomConfig();
            initRoomConfig.seed = currentRoom.seed;
            initRoomConfig.gridSize = currentRoom.gridSize;
            initRoomConfig.roomType = currentRoom.roomType;
            initRoomConfig.offset = currentRoom.offset;
            initRoomConfig.width = currentRoom.width;
            initRoomConfig.internalConfig = currentRoom.internalConfig;
            initRoomConfig.height = currentRoom.height;
            initRoomConfig.doorConfigs = new List<Door>();

            Door initDoorRight = new Door();
            initDoorRight.isConnected = false;
            initDoorRight.wallType = Wall.WallType.Right;
            initRoomConfig.doorConfigs.Add(initDoorRight);

            roomConfigs.Add(initRoomConfig);
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

        RoomConfig newRoomConfig = new RoomConfig();
        newRoomConfig.seed = currentRoom.seed + roomConfigs.Count;
        Random.InitState(newRoomConfig.seed);

        newRoomConfig.width = Random.Range(1, (mapConfig.maxWidth / 2) + 1) * 2 - 1;
        newRoomConfig.height = Random.Range(1, (mapConfig.maxHeight / 2) + 1) * 2 - 1;
        newRoomConfig.offset = new Vector3(
            currentRoom.offset.x + currentRoom.width / 2f + newRoomConfig.width / 2f,
            0,
            currentRoom.offset.y
        );
        newRoomConfig.roomType = RoomConfig.RoomType.Basic;
        newRoomConfig.internalConfig = currentRoom.internalConfig;
        newRoomConfig.gridSize = currentRoom.gridSize;
        newRoomConfig.doorConfigs = new List<Door>();

        Door doorLeft = new Door();
        doorLeft.isConnected = false;
        doorLeft.wallType = Wall.WallType.Left;
        newRoomConfig.doorConfigs.Add(doorLeft);

        Door doorRight = new Door();
        doorRight.isConnected = false;
        doorRight.wallType = Wall.WallType.Right;
        newRoomConfig.doorConfigs.Add(doorRight);

        RoomConfig roomLeft = roomConfigs[roomConfigs.Count - 1];
        roomConfigs.Add(newRoomConfig);
        roomLeft.Connect(doorLeft);
             
    }

    private List<Door> GetUnconnectedDoors(RoomConfig roomConfig)
    {
        List<Door> unconnectedDoors = new List<Door>();
        foreach (Door door in roomConfig.doorConfigs)
            if (!door.isConnected)
                unconnectedDoors.Add(door);
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
