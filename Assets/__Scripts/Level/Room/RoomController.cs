using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif


public class RoomController : MonoBehaviour
{


    [SerializeField]
    RoomManager manager;
    RoomConfig.AnchorType anchorType = RoomConfig.AnchorType.Center;
    Vector2 absOffset = Vector2.zero;

    [SerializeField]
    public RoomConfig roomConfig = new RoomConfig();

    // room scriptable object
    public Room script;

    void OnValidate()
    {
        if (script == null)
        {
            Debug.LogError("RoomController script is null");
            return;
        }
        roomConfig.internalConfig = script.internalConfig;
        roomConfig.gridSize = script.gridSize;
        roomConfig.roomType = script.roomType;
        roomConfig.offset = script.offset;
        roomConfig.width = script.width;
        roomConfig.height = script.height;
        roomConfig.seed = script.seed;
        roomConfig.doorConfigs = script.doorConfigs;
        if(manager != null)
        manager.roomConfig = roomConfig;
    }

    void Start()
    {
        if (gameObject.GetComponent<RoomManager>() == null)
            manager = gameObject.AddComponent<RoomManager>();
        else
            manager = gameObject.GetComponent<RoomManager>();
    }

    public void UpdateConfig(RoomConfig config)
    {
        roomConfig = config;
        manager.roomConfig = roomConfig;
    }

    public void Generate(bool gizmos = false)
    {

        if (anchorType == RoomConfig.AnchorType.Center)
            absOffset = new Vector2(
                -(roomConfig.width - 2) / 2f,
                -roomConfig.height / 2f
            );

        if (!roomConfig.internalConfig)
        {
            Debug.LogWarning("Enemies are not enabled in room config");
            return;
        }

        Random.InitState(roomConfig.seed);
        UnityEngine.Random.InitState(roomConfig.seed);

        GenerateFloor(gizmos);
        GenerateWalls(gizmos);
        GenerateCeiling(gizmos);
        GenerateInteractables(gizmos);
        GenerateEnemies(gizmos);
    }

    // Ceiling will be just a box collider
    void GenerateCeiling(bool gizmos = false)
    {
        float width = roomConfig.width * roomConfig.gridSize;
        float height = roomConfig.height * roomConfig.gridSize;
        Vector3 size = new Vector3(width, 1, height);
        Vector3 ceilingOffset = new Vector3(roomConfig.offset.x * roomConfig.gridSize, 3f, roomConfig.offset.y * roomConfig.gridSize);

        if (gizmos)
        {
            // DrawGizmos(ceilingOffset, Color.red, size, Quaternion.identity);
            return;
        }

        GameObject ceiling = new GameObject("Ceiling");
        ceiling.transform.parent = transform;
        ceiling.AddComponent<BoxCollider>();
        ceiling.transform.localScale = size;
        ceiling.transform.position = ceilingOffset;


    }

    void GenerateEnemies(bool gizmos = false)
    {
        for (int i = 0; i < roomConfig.internalConfig.entityTypes.Count; i++)
        {
            GameObject entityPrefab = roomConfig.internalConfig.entityTypes[i];
            if (entityPrefab != null)
            {
                float x = Random.Range(1, roomConfig.width - 1) + absOffset.x;
                float y = Random.Range(1, roomConfig.height - 1) + absOffset.y;
                Vector3 absPosition = new Vector3(x * roomConfig.gridSize, 0, y * roomConfig.gridSize) + transform.position;
                if (gizmos)
                {
                    DrawGizmos(absPosition, Color.red, Vector3.one, Quaternion.identity);
                    continue;
                }

                GameObject entityInstance = Instantiate(entityPrefab, absPosition, Quaternion.identity);

                EntityController entityController = entityInstance.GetComponent<EntityController>();
                manager?.watcher?.entities?.Add(entityInstance.GetComponent<EntityController>());
                
                if (entityController == null)
                {
                    Debug.LogError("EntityController is null");
                    continue;
                }


                entityInstance.transform.parent = transform;
                entityInstance.name = $"Enemy_{x}_{y}";
                entityInstance.transform.localPosition = new Vector3(x * roomConfig.gridSize, 0, y * roomConfig.gridSize);
                entityInstance.transform.localRotation = Quaternion.identity;
                entityInstance.transform.localScale = Vector3.one;


            }
        }
    }

    void GenerateInteractables(bool gizmos = false)
    {
        // Calculate the number of interactables based on the room's size
        int interactableCount = roomConfig.width * roomConfig.height / 10; // Adjust this value as needed

        // Spread interactables randomly across the room
        for (int i = 0; i < interactableCount; i++)
        {
            // Randomly select an interactable prefab
            GameObject interactablePrefab = roomConfig.internalConfig.interactableTypes[Random.Range(0, roomConfig.internalConfig.interactableTypes.Count)];

            // Randomly calculate the position within the room
            float x = Random.Range(1, roomConfig.width - 1) + absOffset.x;
            float y = Random.Range(1, roomConfig.height - 1) + absOffset.y;
            Vector3 absPosition = new Vector3(x * roomConfig.gridSize, 0, y * roomConfig.gridSize) + transform.position;

            if (gizmos)
            {
                // Draw interactable gizmos
                DrawGizmos(absPosition, Color.yellow, Vector3.one, Quaternion.identity);
            }
            else
            {
                // Instantiate and place the interactable prefab
                GameObject interactableInstance = Instantiate(interactablePrefab, absPosition, Quaternion.identity);
                interactableInstance.transform.SetParent(transform);
                interactableInstance.name = $"Interactable_{x}_{y}";
            }
        }
    }

    void GenerateWalls(bool gizmos = false)
    {
        for (int x = -1; x <= roomConfig.width; x++)
        {
            for (int y = -1; y <= roomConfig.height; y++)
            {
                Vector3 absPosition = new Vector3(((float)x + (float)absOffset.x) * roomConfig.gridSize - roomConfig.gridSize / 2f, 0, ((float)y + (float)absOffset.y) * roomConfig.gridSize + roomConfig.gridSize / 2f);
                absPosition += transform.position;
                Vector2Int gridPosition = new Vector2Int(x, y);

                Wall.WallType? wallType = RoomConfig.GetWallType(roomConfig, gridPosition);


                if (!wallType.HasValue) continue;

#if UNITY_EDITOR
                    if (gizmos)
                    {
                        DrawGizmos(absPosition, Color.blue, new Vector3(1f, 1f, 1f), Quaternion.identity);
                        string text = wallType.Value == Wall.WallType.Left ? "Left" : (wallType.Value == Wall.WallType.Right ? "Right" : (wallType.Value == Wall.WallType.Top ? "Top" : "Bottom"));
                        text = x + ", " + y + "\n" + text;
                        // Draw the text gizmo at the gridPosition of the GameObject
                        Handles.Label(absPosition + new Vector3(-1, 2, 0), text);
                    }
#endif

                switch (wallType.Value)
                {
                    case Wall.WallType.Left:
                        absPosition += new Vector3(roomConfig.gridSize / 2f, 0, 0);
                        break;
                    case Wall.WallType.Right:
                        absPosition -= new Vector3(roomConfig.gridSize / 2f, 0, 0);
                        break;
                    case Wall.WallType.Bottom:
                        absPosition += new Vector3(0, 0, roomConfig.gridSize / 2f);
                        break;
                    case Wall.WallType.Top:
                        absPosition -= new Vector3(0, 0, roomConfig.gridSize / 2f);
                        break;
                }

                if (RoomConfig.IsDoor(roomConfig, gridPosition, wallType.Value))
                {
                    Door newDoor = new Door();
                    newDoor.wallType = wallType.Value;
                    newDoor.gridPosition = gridPosition;
                    newDoor.absPosition = absPosition;

                    GenerateDoor(newDoor, gizmos);
                }
                else
                {
                    GenerateWall(absPosition, wallType.Value, gridPosition, gizmos);
                }
            }
        }
    }


    void GenerateFloor(bool gizmos = false)
    {
        Color darkGray = new Color(0.05f, 0.05f, 0.05f);
        // Generate the floor tiles
        for (int x = 0; x < roomConfig.width; x++)
        {
            for (int y = 0; y < roomConfig.height; y++)
            {
                GameObject floorPrefab = roomConfig.internalConfig.floorTypes[Random.Range(0, roomConfig.internalConfig.floorTypes.Count)];
                Vector3 absPosition = new Vector3((x + absOffset.x) * roomConfig.gridSize, 0, (y + absOffset.y) * roomConfig.gridSize);
                absPosition += transform.position;
                Quaternion rotation = Quaternion.identity;

                if (gizmos)
                {
                    Vector3 scale = new Vector3(roomConfig.gridSize, 1f, roomConfig.gridSize);
                    absPosition += new Vector3(-roomConfig.gridSize / 2f, 0, roomConfig.gridSize / 2f);
                    DrawGizmos(absPosition, darkGray, scale, rotation);
                }
                else
                {
                    if (!Application.isPlaying) return;
                    GameObject floorInstance = Instantiate(floorPrefab, absPosition, rotation);
                    floorInstance.transform.SetParent(transform);
                    floorInstance.name = $"Floor_{x}_{y}";
                }
            }
        }
    }

    void GenerateWall(Vector3 absPosition, Wall.WallType wallType, Vector2Int gridPosition, bool gizmos = false)
    {
        if (gizmos)
        {
            DrawGizmos(absPosition, Color.gray, new Vector3(roomConfig.gridSize, roomConfig.gridSize, 1f), Wall.GetRotation(wallType));
        }
        else if (Application.isPlaying)
        {
            GameObject wallPrefab = roomConfig.internalConfig.wallTypes[Random.Range(0, roomConfig.internalConfig.wallTypes.Count)];
            Vector3 wallPosition = absPosition + Wall.GetOffset(wallType, roomConfig.gridSize);
            Quaternion wallRotation = Wall.GetRotation(wallType);

            GameObject wallInstance = Instantiate(wallPrefab, wallPosition, wallRotation);
            wallInstance.transform.SetParent(transform);
            wallInstance.name = $"Wall_{gridPosition.x}_{gridPosition.y}";
        }
    }

    void GenerateDoor(Door door, bool gizmos = false)
    {
        if (gizmos)
        {
            DrawGizmos(door.absPosition, Color.red, new Vector3(roomConfig.gridSize, roomConfig.gridSize, 1f), Door.GetRotation(door.wallType));
            return;
        }

        GameObject doorPrefab = roomConfig.getDoorPrefab(door.wallType);

        int centerPosX = roomConfig.width / 2;
        int centerPosY = roomConfig.height / 2;

        if (roomConfig.height % 2 == 0 && (door.wallType == Wall.WallType.Left || door.wallType == Wall.WallType.Right))
        {
            doorPrefab = roomConfig.getDoubleDoorPrefab(door.wallType);
            if (door.gridPosition.y == (centerPosY))
                if (door.wallType == Wall.WallType.Left)
                    door.absPosition += new Vector3(0, 0, -2 * roomConfig.gridSize + (roomConfig.gridSize / 2));
                else if (door.wallType == Wall.WallType.Right)
                    door.absPosition += new Vector3(0, 0, roomConfig.gridSize / 2);
                else return;
            else return;
        }
        else if (roomConfig.width % 2 == 0 && (door.wallType == Wall.WallType.Top || door.wallType == Wall.WallType.Bottom))
        {
            doorPrefab = roomConfig.getDoubleDoorPrefab(door.wallType);
            if (door.gridPosition.x == (centerPosX))
                if (door.wallType == Wall.WallType.Top)
                    door.absPosition += new Vector3(-2 * roomConfig.gridSize + (roomConfig.gridSize / 2), 0, 0);
                else if (door.wallType == Wall.WallType.Bottom)
                    door.absPosition += new Vector3(roomConfig.gridSize / 2, 0, 0);
                else return;
            else return;
        }

        if (!Application.isPlaying) return;
        GameObject doorInstance = Instantiate(doorPrefab);
        doorInstance.transform.position = door.absPosition;
        doorInstance.transform.rotation = Door.GetRotation(door.wallType);
        doorInstance.transform.SetParent(transform);
        doorInstance.name = $"Door_{door.gridPosition.x}_{door.gridPosition.y}";
        DoorController doorController = doorInstance.GetComponent<DoorController>();
        doorController.door = door;
        doorController.isLocked = true;
        manager.watcher.doors.Add(doorController);
    }   

    void DrawGizmos(Vector3 absPosition, Color color, Vector3 scale, Quaternion rotation)
    {
#if UNITY_EDITOR
            Gizmos.color = color;
            Gizmos.matrix = Matrix4x4.TRS(absPosition, rotation, scale);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity;
#endif
    }

    void OnDrawGizmos()
    {
        Generate(gizmos: true);

    }

}
