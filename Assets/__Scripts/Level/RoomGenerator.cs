using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomGenerator : MonoBehaviour
{
    public List<GameObject> wallTypes;
    public List<GameObject> floorTypes;
    public List<GameObject> singleDoorTypes;
    public List<GameObject> doubleDoorTypes;
    public int gridSize = 5;

    [Range(1, 100)]
    public int width = 2;
    [Range(1, 100)]
    public int height = 2;

    public bool hasDoorUp = false;
    public bool hasDoorDown = false;
    public bool hasDoorRight = false;
    public bool hasDoorLeft = false;

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
    }

    void GenerateFloor(bool gizmos = false)
    {
        Color darkGray = new Color(0.05f, 0.05f, 0.05f);
        // Generate the floor tiles
        for (int x = 0; x < width * gridSize; x += gridSize)
        {
            for (int y = 0; y < height * gridSize; y += gridSize)
            {
                GameObject floorPrefab = floorTypes[Random.Range(0, floorTypes.Count)];
                Vector3 position = new Vector3(x, 0, y );
                Quaternion rotation = Quaternion.identity;

                if (gizmos)
                {
                    Vector3 scale = new Vector3(gridSize, 1f, gridSize);
                    position += new Vector3(- gridSize / 2f, 0, gridSize / 2f);
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
                Vector2Int gridPosition = new Vector2Int(x * gridSize, y * gridSize);
                Vector2Int position = new Vector2Int(x, y);

                Vector3 pos = new Vector3(gridPosition.x, 0, gridPosition.y) + new Vector3(-gridSize / 2f, 0, gridSize / 2f);
                WallType? wallType = GetWallType(position);
                

                if (!wallType.HasValue) continue;

                if (gizmos) {
                    DrawGizmos(pos, Color.blue, new Vector3(1f, 1f, 1f), Quaternion.identity);
                    string text = wallType.Value == WallType.Left? "Left" : (wallType.Value == WallType.Right? "Right" : (wallType.Value == WallType.Top? "Top" : "Bottom"));
                    text = x + ", " + y + "\n" + text; 
                    // Draw the text gizmo at the position of the GameObject
                    Handles.Label(pos + new Vector3(-2, 0, 0), text);
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
                    GenerateDoor(pos, wallType.Value, gizmos);
                }
                else
                {
                    GenerateWall(pos, wallType.Value, gizmos);
                }
            }
        }
    }

    void GenerateDoor(Vector3 position, WallType wallType, bool gizmos = false)
    {
        GameObject doorPrefab = singleDoorTypes[Random.Range(0, singleDoorTypes.Count)];
        Quaternion doorRotation = Quaternion.identity;
        Vector3 doorPosition = position;
        Vector3 doorScale = new Vector3(1f, gridSize, gridSize);

        if (gizmos)
        {  
            switch (wallType)
            {
                case WallType.Left:
                    break;
                case WallType.Right:
                    break;
                case WallType.Top:
                    doorScale = new Vector3(gridSize, gridSize, 1f);
                    break;
                case WallType.Bottom:
                    doorScale = new Vector3(gridSize, gridSize, 1f);
                    break;
                default:
                    Debug.LogError("Unknown wall type");
                    break;
            }
            DrawGizmos(doorPosition, Color.red, doorScale, doorRotation);
        }
        else
        {
            Instantiate(doorPrefab, doorPosition, doorRotation);
        }
    }

    void GenerateWall(Vector3 position, WallType wallType, bool gizmos = false)
    {
        GameObject wallPrefab = wallTypes[Random.Range(0, wallTypes.Count)];
        Quaternion wallRotation = Quaternion.identity;
        Vector3 wallPosition = position;

        Vector3 wallScale = new Vector3(1f, gridSize, gridSize);

        switch (wallType)
        {
            case WallType.Left:
                break;
            case WallType.Right:
                break;
            case WallType.Top:
                wallScale = new Vector3(gridSize, gridSize, 1f);
                break;
            case WallType.Bottom:
                wallScale = new Vector3(gridSize, gridSize, 1f);
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
