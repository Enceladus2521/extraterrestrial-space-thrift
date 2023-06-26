using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class RoomGenerator : MonoBehaviour
{
    public List<GameObject> wallTypes;
    public List<GameObject> floorTypes;
    public List<GameObject> singleDoorTypes;
    public List<GameObject> doubleDoorTypes;
    
    public List<GameObject> interactableTypes;

    public int gridSize = 5;

    [Range(1, 100)]
    public int width = 2;
    [Range(1, 100)]
    public int height = 2;

    public bool hasDoorUp = false;
    public bool hasDoorDown = false;
    public bool hasDoorRight = false;
    public bool hasDoorLeft = false;

    public int offsetX = 0;
    public int offsetY = 0;

    public enum RoomType
    {
        Start,
        Boss,
        Medic,
        Shop
    }
    
    public RoomType roomType = RoomType.Start;

    public enum WallType
    {
        Left,
        Right,
        Top,
        Bottom
    }


    void Start()
    {
        GenerateRoom();
    }

    void GenerateRoom()
    {
        GenerateFloor();
        GenerateWalls();
    }

    public void Generate(bool gizmos = false)
    {
        GenerateFloor(gizmos);
        GenerateWalls(gizmos);
        GenerateInteractables(gizmos);
    }

    void GenerateInteractables(bool gizmos = false)
    {
       
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
                Vector3 position = new Vector3((x + offsetX) * gridSize, 0, (y + offsetY) * gridSize);
                Quaternion rotation = Quaternion.identity;

                if (gizmos)
                {
                    Vector3 scale = new Vector3(gridSize, 1f, gridSize);
                    position += new Vector3(-gridSize / 2f, 0, gridSize / 2f);
                    DrawGizmos(position, darkGray, scale, rotation);
                }
                else
                {   
                    Instantiate(floorPrefab, position, rotation);
                }
            }
        }
    }


    WallType? GetWallType(Vector2Int position)
    {
        if (position.x == -1 && position.y >= 0 && position.y < height)
            return WallType.Left;

        if (position.x == width && position.y >= 0 && position.y < height)
            return WallType.Right;

        if (position.y == -1 && position.x >= 0 && position.x < width)
            return WallType.Bottom;

        if (position.y == height && position.x >= 0 && position.x < width)
            return WallType.Top;

        return null;
    }

    bool IsDoor(Vector2Int position, WallType wallType)
    {
        int centerPosX = width / 2;
        int centerPosY = height / 2;

        if (position.y >= 0 && position.y < height)
        {
            if (position.x == -1 && wallType == WallType.Left)
                return (hasDoorLeft && (position.y == centerPosY ||(height % 2 == 0 && position.y == centerPosY - 1)));

            if (position.x == width && wallType == WallType.Right)
                return (hasDoorRight && (position.y == centerPosY || (height % 2 == 0 && position.y == centerPosY - 1 )));
        }

        if (position.x >= 0 && position.x < width)
        {
            if (position.y == -1 && wallType == WallType.Bottom)
                return (hasDoorDown && (position.x == centerPosX || (width % 2 == 0 && position.x == centerPosX - 1)));

            if (position.y == height && wallType == WallType.Top)
                return (hasDoorUp && (position.x == centerPosX || (width % 2 == 0 && position.x == centerPosX - 1 )));
        }

        return false;
    }

    void GenerateWalls(bool gizmos = false)
    {
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                Vector2Int gridPosition = new Vector2Int((x + offsetX) * gridSize, (y + offsetY) * gridSize);
                Vector2Int position = new Vector2Int(x, y);

                Vector3 pos = new Vector3(gridPosition.x, 0, gridPosition.y) + new Vector3(-gridSize / 2f, 0, gridSize / 2f);
                WallType? wallType = GetWallType(position);
                

                if (!wallType.HasValue) continue;

                if (gizmos) {
                    DrawGizmos(pos, Color.blue, new Vector3(1f, 1f, 1f), Quaternion.identity);
                    string text = wallType.Value == WallType.Left? "Left" : (wallType.Value == WallType.Right? "Right" : (wallType.Value == WallType.Top? "Top" : "Bottom"));
                    text = x + ", " + y + "\n" + text; 
                    // Draw the text gizmo at the position of the GameObject
                    Handles.Label(pos + new Vector3(-1, 2, 0), text);
                }

                switch (wallType.Value)
                {
                    case WallType.Left:
                        pos += new Vector3(gridSize / 2f, 0, 0);
                        break;
                    case WallType.Right:
                        pos -= new Vector3(gridSize / 2f, 0, 0);
                        break;
                    case WallType.Bottom:
                        pos += new Vector3(0, 0, gridSize / 2f);
                        break;
                    case WallType.Top:
                        pos -= new Vector3(0, 0, gridSize / 2f);
                        break;
                }

                if (IsDoor(position, wallType.Value))
                {
                    GenerateDoor(pos, wallType.Value, position, gizmos);
                }
                else
                {
                    GenerateWall(pos, wallType.Value, gizmos);
                }
            }
        }
    }

    void GenerateDoor(Vector3 pos, WallType wallType, Vector2Int position, bool gizmos = false)
    {
        GameObject doorPrefab = singleDoorTypes[Random.Range(0, singleDoorTypes.Count)];
        Quaternion doorRotation = Quaternion.identity;
        Vector3 doorPosition = pos;
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
                    if (height % 2 == 0 && (position.y == (centerPosY)))
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
                    if (width % 2 == 0 && (position.x == centerPosX))
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

            Instantiate(doorPrefab, doorPosition, doorRotation);
        }
    }

    void GenerateWall(Vector3 position, WallType wallType, bool gizmos = false)
    {
        GameObject wallPrefab = wallTypes[Random.Range(0, wallTypes.Count)];
        Quaternion wallRotation = Quaternion.identity;
        Vector3 wallPosition = position;

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
            Instantiate(wallPrefab, wallPosition, wallRotation);
        }
    }

    void DrawGizmos(Vector3 position, Color color, Vector3 scale, Quaternion rotation)
    {
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, scale);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
    }

    void OnDrawGizmos()
    {
        Generate(gizmos: true);
    }
}
