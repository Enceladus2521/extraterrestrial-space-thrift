using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour
{

    private float roomGenerationDistance = 30f;
    public static LevelManager Instance { get; private set; }

    private LevelController controller;
    public Watcher watcher;
    private Watcher voidWatcher;

    private Vector3 mostLeftPlayerPos = Vector3.zero;
    private Vector3 mostRightPlayerPos = Vector3.zero;
    public Vector3 MostLeftPlayerPos { get { return mostLeftPlayerPos; } }
    public Vector3 MostRightPlayerPos { get { return mostRightPlayerPos; } }

    // Level = LevelController
    public GameObject Level { get { return controller.gameObject; } }

    

    // int TopDifficulty from controller.roomsGenerated which are unlocked
    public int GetTopDifficulty()
    {
        int topDifficulty = 0;
        for (int i = 0; i < controller.RoomsGenerated.Count; i++)
        {
            RoomManager room = controller.RoomsGenerated[i];
            if (!room.IsLocked)
                if (room.Difficulty > topDifficulty)
                    topDifficulty = room.Difficulty;
        }
        return topDifficulty;
    }

    void Awake()
    {
        if (LevelManager.Instance != null && LevelManager.Instance != this)
        {
            Destroy(this.gameObject);
        }

        Instance = this;
        watcher = new Watcher();
        voidWatcher = new Watcher();
        if (gameObject.GetComponent<LevelController>() == null)
        {
            controller = gameObject.AddComponent<LevelController>();
        }
        else
        {
            controller = gameObject.GetComponent<LevelController>();
        }
    }

    void Start()
    {
        controller.GenerateNewRoomConfig();
    }


    public List<EnemyController> Entities { get { return watcher.entities; } }

    public List<Interacter> GetInteractables()
    {
        return watcher.interactables;
    }

    public DoorController GetClosestDoor(DoorController controller)
    {
        // Find closest door by tag "door"
        if (watcher.doors.Count == 0)
        {
            Debug.Log("no doors found in game");
            return null;
        }
        DoorController closestDoor = null;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < watcher.doors.Count; i++)
        {
            DoorController otherDoor = watcher.doors[i].GetComponent<DoorController>();
            // check if its not the same door, other door will have the opposite wall type
            if (otherDoor != null)
            {
                if (otherDoor.gameObject == controller.gameObject)
                {
                    continue;
                }


                float distance = Vector3.Distance(controller.transform.position, otherDoor.transform.position);


                if (distance < closestDistance)
                {
                    closestDoor = otherDoor;
                    closestDistance = distance;
                }
            }
            else
            {
                Debug.Log("other door is null");
            }

        }

        return closestDoor;
    }

    public void Update()
    {
        if (controller == null) return;

        Vector3 avgPlayerPos = new Vector3(0f, 0f, 0f);
        List<GameObject> players = GameManager.Instance?.Players;

        if (players != null && players.Count > 0)
        {
            for (int i = 0; i < players.Count; i++)
                avgPlayerPos += players[i].transform.position;
            avgPlayerPos /= players.Count;

            // Update most left and most right player positions
            mostLeftPlayerPos = players[0].transform.position;
            mostRightPlayerPos = players[0].transform.position;

            for (int i = 1; i < players.Count; i++)
            {
                GameObject player = players[i];
                if (player.transform.position.x < mostLeftPlayerPos.x)
                    mostLeftPlayerPos = player.transform.position;
                if (player.transform.position.x > mostRightPlayerPos.x)
                    mostRightPlayerPos = player.transform.position;
            }
        }
        else
        {
            // No players in the scene, reset positions
            mostLeftPlayerPos = Vector3.zero;
            mostRightPlayerPos = Vector3.zero;
        }

        float distance = Vector3.Distance(avgPlayerPos, controller.LastRoomPos);

        if (distance < roomGenerationDistance)
        {
            controller.GenerateNewRoomConfig();
        }

        UpdateLevel();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(controller?.LastRoomPos ?? Vector3.zero, roomGenerationDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(mostLeftPlayerPos, 1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mostRightPlayerPos, 1f);
    }



    public void UpdateLevel()
    {

        // Loop through all generated rooms to activate/deactivate based on rendering
        for (int i = 0; i < controller.RoomGeneratedCount; i++)
        {

            RoomManager roomManager = controller.RoomsGenerated[i];
            if (roomManager == null) continue;
            bool shouldRender = IsRoomInRenderingBounds(roomManager);

            // Set the room's GameObject active state based on rendering bounds
            if (roomManager?.gameObject.activeSelf != shouldRender)
                roomManager?.gameObject.SetActive(shouldRender);
        }

        if (controller.RoomGeneratedCount == controller.RoomCount)
            return;

        if (controller.RoomGeneratedCount < controller.RoomCount)
        {
            controller.GenerateNewRoom();
            return;
        }

        if (controller.RoomGeneratedCount < controller.RoomCount - 6)
            return;

        if (controller.RoomGeneratedCount > controller.RoomCount)
        {
            Debug.Log("Regenerating level");
            controller.Clear();
            controller.GenerateNewRoomConfig();
            return;
        }
    }
    private bool IsRoomInRenderingBounds(RoomManager roomManager)
    {
        if (roomManager == null) return false;
        if (roomManager.RoomConfig == null) return false;
        // Calculate the room's bounds
        float halfRoomWidth = roomManager.RoomConfig.width * roomManager.RoomConfig.gridSize * 0.5f;
        float halfRoomHeight = roomManager.RoomConfig.height * roomManager.RoomConfig.gridSize * 0.5f;
        Vector3 roomCenter = roomManager.transform.position;
        Bounds roomBounds = new Bounds(roomCenter, new Vector3(halfRoomWidth * 2, 1f, halfRoomHeight * 2));

        // Calculate the bounds of the rendering area with an offset of 42 units on both sides
        float halfRenderWidth = Mathf.Abs(mostRightPlayerPos.x - mostLeftPlayerPos.x) * 0.5f + roomGenerationDistance;
        Vector3 renderCenter = (mostLeftPlayerPos + mostRightPlayerPos) * 0.5f;
        Bounds renderBounds = new Bounds(renderCenter, new Vector3(halfRenderWidth * 2, 1f, 100f)); // Adjust the Z value (100f) to cover the height of the room, assuming it's a 2D game.
        // Check if the room's bounds intersect with the rendering bounds
        return roomBounds.Intersects(renderBounds);
    }

}
