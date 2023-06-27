using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class RoomConfig
{
    public float gridSize;
    public RoomController.RoomType roomType;
    public Vector2 offset;
    public int width;
    public int height;
    public int seed;
    public Transform transform;
    public List<DoorConfig> doorConfigs = new List<DoorConfig>();

    public RoomConfig Empty() => new RoomConfig
    {
        roomType = RoomController.RoomType.Start,
        offset = Vector2.zero,
        width = 1,
        height = 1,
        gridSize = 5,
        transform = null,
        seed = 42,
        doorConfigs = new List<DoorConfig>()
    };
}

[System.Serializable]
public class DoorConfig
{
    public RoomController.WallType attachedWall;
    public bool isConnected;
}

public class RoomController : MonoBehaviour
{
    public List<GameObject> wallTypes;
    public List<GameObject> floorTypes;
    public List<GameObject> singleDoorTypes;
    public List<GameObject> doubleDoorTypes;
    public List<GameObject> interactableTypes;

    public float gridSize = 5f;

    [Range(1, 100)]
    public int width = 2;
    [Range(1, 100)]
    public int height = 2;
    public int seed = 42;

    public List<DoorConfig> doorConfigs = new List<DoorConfig>();

    public Vector2 offset = Vector2.zero;
    Vector2 absOffset = Vector2.zero;

    public enum RoomType
    {
        Start,
        Boss,
        Basic,
        Medic,
        Shop
    }
    
    public AnchorType anchorType = AnchorType.Bottom;
    public RoomType roomType = RoomType.Start;

    public enum WallType
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum AnchorType
    {
        Bottom,
        Center
    }

    public RoomConfig roomConfig()
    {
        RoomConfig roomConfig = new RoomConfig();
        roomConfig.width = width;
        roomConfig.height = height;
        roomConfig.offset = offset;
        roomConfig.roomType = roomType;
        roomConfig.doorConfigs = doorConfigs;
        roomConfig.transform = transform;
        roomConfig.gridSize = gridSize;
        roomConfig.seed = seed;
        return roomConfig;

    }

    void Start()
    {
        Random.InitState(seed);
        // GenerateRoom();
    }

    public void UpdateConfig(RoomConfig config)
    {
        if (config == null)
        {
            // If no configuration is provided, use the Empty() method of RoomConfig
            config = new RoomConfig().Empty();
        }

        width = config.width;
        height = config.height;
        offset = config.offset;
        gridSize = config.gridSize;
        roomType = config.roomType;
        doorConfigs = config.doorConfigs;
        transform.position = config.transform.position;
        seed = config.seed;
        
    }

    public void Generate(bool gizmos = false)
    {        

        if (anchorType == AnchorType.Center)
            absOffset = offset + new Vector2(
                - (width - 2) / 2f,
                - height / 2f
            );
            
        GenerateFloor(gizmos);
        GenerateWalls(gizmos);
        GenerateInteractables(gizmos);
    }


    void GenerateInteractables(bool gizmos = false)
        {
            // Calculate the number of interactables based on the room's size
            int interactableCount = width * height / 10; // Adjust this value as needed

            // Spread interactables randomly across the room
            for (int i = 0; i < interactableCount; i++)
            {
                // Randomly select an interactable prefab
                GameObject interactablePrefab = interactableTypes[Random.Range(0, interactableTypes.Count)];

                // Randomly calculate the position within the room
                float x = Random.Range(1, width - 1) + absOffset.x;
                float y = Random.Range(1, height -1) + absOffset.y;
                Vector3 absPosition = new Vector3(x * gridSize, 0, y * gridSize) + transform.position;

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

    WallType? GetWallType(Vector2Int gridPosition)
    {
        if (gridPosition.x == -1 && gridPosition.y >= 0 && gridPosition.y < height)
            return WallType.Left;

        if (gridPosition.x == width && gridPosition.y >= 0 && gridPosition.y < height)
            return WallType.Right;

        if (gridPosition.y == -1 && gridPosition.x >= 0 && gridPosition.x < width)
            return WallType.Bottom;

        if (gridPosition.y == height && gridPosition.x >= 0 && gridPosition.x < width)
            return WallType.Top;

        return null;
    }

    bool hasDoorOnSide(WallType wallType)
    {
        bool hasDoor = false;

        for (int i = 0; i < doorConfigs.Count; i++)
        {
            if (doorConfigs[i].attachedWall == wallType)
                hasDoor = true;
        }

        return hasDoor;

    }

    bool IsDoor(Vector2Int position, WallType wallType)
    {
        int centerPosX = width / 2;
        int centerPosY = height / 2;

        bool hasDoor = hasDoorOnSide(wallType);

        if (position.y >= 0 && position.y < height)
        {
            if (position.x == -1 && wallType == WallType.Left)
                return (hasDoor && (position.y == centerPosY ||(height % 2 == 0 && position.y == centerPosY - 1)));

            if (position.x == width && wallType == WallType.Right)
                return (hasDoor && (position.y == centerPosY || (height % 2 == 0 && position.y == centerPosY - 1 )));
        }

        if (position.x >= 0 && position.x < width)
        {
            if (position.y == -1 && wallType == WallType.Bottom)
                return (hasDoor && (position.x == centerPosX || (width % 2 == 0 && position.x == centerPosX - 1)));

            if (position.y == height && wallType == WallType.Top)
                return (hasDoor && (position.x == centerPosX || (width % 2 == 0 && position.x == centerPosX - 1 )));
        }

        return false;
    }

    void GenerateWalls(bool gizmos = false)
    {
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                Vector3 absPosition = new Vector3(((float)x + (float)absOffset.x) * gridSize - gridSize / 2f, 0, ((float)y + (float)absOffset.y) * gridSize + gridSize / 2f);
                absPosition += transform.position;
                Vector2Int gridPosition = new Vector2Int(x, y);

                WallType? wallType = GetWallType(gridPosition);
                

                if (!wallType.HasValue) continue;

                if (gizmos) {
                    DrawGizmos(absPosition, Color.blue, new Vector3(1f, 1f, 1f), Quaternion.identity);
                    string text = wallType.Value == WallType.Left? "Left" : (wallType.Value == WallType.Right? "Right" : (wallType.Value == WallType.Top? "Top" : "Bottom"));
                    text = x + ", " + y + "\n" + text; 
                    // Draw the text gizmo at the gridPosition of the GameObject
                    Handles.Label(absPosition + new Vector3(-1, 2, 0), text);
                }

                switch (wallType.Value)
                {
                    case WallType.Left:
                        absPosition += new Vector3(gridSize / 2f, 0, 0);
                        break;
                    case WallType.Right:
                        absPosition -= new Vector3(gridSize / 2f, 0, 0);
                        break;
                    case WallType.Bottom:
                        absPosition += new Vector3(0, 0, gridSize / 2f);
                        break;
                    case WallType.Top:
                        absPosition -= new Vector3(0, 0, gridSize / 2f);
                        break;
                }

                if (IsDoor(gridPosition, wallType.Value))
                {
                    GenerateDoor(absPosition, wallType.Value, gridPosition, gizmos);
                }
                else
                {
                    GenerateWall(absPosition, wallType.Value, gridPosition, gizmos);
                }
            }
        }
    }

    void GenerateDoor(Vector3 absPosition, WallType wallType, Vector2Int gridPosition, bool gizmos = false)
    {
        GameObject doorPrefab = singleDoorTypes[Random.Range(0, singleDoorTypes.Count)];
        Quaternion doorRotation = Quaternion.identity;
        Vector3 doorPosition = absPosition;
        Vector3 doorScale = new Vector3(gridSize, gridSize, 1f);

        switch (wallType)
        {
            case WallType.Left:
                doorRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case WallType.Right:
                doorRotation = Quaternion.Euler(new Vector3(0, -90, 0));
                break;
            case WallType.Top:
                doorRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case WallType.Bottom:
                break;
            default:
                Debug.LogError("Unknown wall type");
                break;
        }

        if (gizmos)
        {  
            DrawGizmos(doorPosition, Color.red, doorScale, doorRotation);
        }
        else
        {
            int centerPosX = width / 2;
            int centerPosY = height / 2;
                
            if ( height % 2 == 0 || width % 2 == 0){
                if (wallType == WallType.Left || wallType == WallType.Right)
                    if (height % 2 == 0 && (gridPosition.y == (centerPosY)))
                    {
                        if (wallType == WallType.Left)
                            doorPosition += new Vector3(0, 0, -2 * gridSize + (gridSize / 2));
                        if (wallType == WallType.Right)
                            doorPosition += new Vector3(0, 0, gridSize / 2 );
                        doorPrefab = doubleDoorTypes[Random.Range(0, doubleDoorTypes.Count)];
                    }
                    else 
                    {
                        return;
                    }
                else if (wallType == WallType.Top || wallType == WallType.Bottom)
                    if (width % 2 == 0 && (gridPosition.x == centerPosX))
                    {
                        if (wallType == WallType.Top)
                            doorPosition += new Vector3(-2 * gridSize + (gridSize / 2), 0, 0);
                        if (wallType == WallType.Bottom)
                            doorPosition += new Vector3(gridSize / 2 , 0, 0);
                        doorPrefab = doubleDoorTypes[Random.Range(0, doubleDoorTypes.Count)];
                    }
                    else
                    {
                        return;
                    }
            }

            if (!Application.isPlaying) return;
            GameObject doorInstance = Instantiate(doorPrefab, doorPosition, doorRotation);
            doorInstance.transform.SetParent(transform);
            doorInstance.name = $"Door_{gridPosition.x}_{gridPosition.y}";
        }
    }


    void GenerateFloor(bool gizmos = false)
    {
        Color darkGray = new Color(0.05f, 0.05f, 0.05f);
        // Generate the floor tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject floorPrefab = floorTypes[Random.Range(0, floorTypes.Count)];
                Vector3 absPosition = new Vector3((x + absOffset.x) * gridSize, 0, (y + absOffset.y) * gridSize);
                absPosition += transform.position;
                Quaternion rotation = Quaternion.identity;

                if (gizmos)
                {
                    Vector3 scale = new Vector3(gridSize, 1f, gridSize);
                    absPosition += new Vector3(-gridSize / 2f, 0, gridSize / 2f);
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

    void GenerateWall(Vector3 absPosition, WallType wallType, Vector2Int gridPosition, bool gizmos = false)
    {
        GameObject wallPrefab = wallTypes[Random.Range(0, wallTypes.Count)];
        Quaternion wallRotation = Quaternion.identity;
        Vector3 wallPosition = absPosition;

        Vector3 wallScale = new Vector3(gridSize, gridSize, 1f);

        switch (wallType)
        {
            case WallType.Left:
                wallRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case WallType.Right:
                wallRotation = Quaternion.Euler(new Vector3(0, -90, 0));
                break;
            case WallType.Top:
                wallRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case WallType.Bottom:
                break;
            default:
                Debug.LogError("Unknown wall type");
                break;
        }

        if (gizmos)
        {
            DrawGizmos(wallPosition, Color.gray, wallScale, wallRotation);
        }
        else
        {
            switch (wallType)
            {
                case WallType.Top:
                    wallPosition += new Vector3(-gridSize / 2f, 0, 0);
                    break;
                case WallType.Bottom:
                    wallPosition += new Vector3( gridSize / 2f, 0, 0);
                    break;
                case WallType.Left:
                    wallPosition += new Vector3(0, 0, -gridSize / 2f);
                    break;
                case WallType.Right:
                    wallPosition += new Vector3(0, 0, gridSize / 2f);
                    break;
                default:
                    wallPosition += new Vector3(0, 0, 0);
                    break;
            }
            if (!Application.isPlaying) return;
            GameObject wallInstance = Instantiate(wallPrefab, wallPosition, wallRotation);
            wallInstance.transform.SetParent(transform);
            wallInstance.name = $"Wall_{gridPosition.x}_{gridPosition.y}";
        }
    }

    void DrawGizmos(Vector3 absPosition, Color color, Vector3 scale, Quaternion rotation)
    {
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(absPosition, rotation, scale);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }

    void OnDrawGizmos()
    {
        Random.InitState(seed);
        Generate(gizmos: true);

    }
}
