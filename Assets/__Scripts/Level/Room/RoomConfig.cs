using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class RoomConfig
{
    public RoomInternal internalConfig;
    public float gridSize;
    public RoomType roomType;
    public Vector2 offset;
    public int width;
    public int height;
    public int seed;
    public List<Door> doorConfigs;

    public void Connect(Door door)
    {
        if (door.isConnected)
            return;

        Wall.WallType wallType = door.wallType;
        // get opposite wall type of the door
        Wall.WallType oppositeWallType = wallType == Wall.WallType.Left ? Wall.WallType.Right : Wall.WallType.Left;
        // find own door with opposite wall type
        Door otherDoor = doorConfigs.Find(d => d.wallType == oppositeWallType);
        if (otherDoor == null)
            return;

        otherDoor.isConnected = true;
        door.isConnected = true;
    }

    public enum AnchorType
    {
        Bottom,
        Center
    }

    public enum RoomType
    {
        Start,
        Boss,
        Basic,
        Medic,
        Shop
    }

    public GameObject getDoorPrefab(Wall.WallType wallType)
    {
        return internalConfig.singleDoorTypes[Random.Range(0, internalConfig.singleDoorTypes.Count)];
    }

    public GameObject getDoubleDoorPrefab(Wall.WallType wallType)
    {
        return internalConfig.doubleDoorTypes[Random.Range(0, internalConfig.doubleDoorTypes.Count)];
    }

    public Vector3 getDoorPosition(Vector3 absPosition, Wall.WallType wallType, Vector2Int gridPosition)
    {
        Vector3 doorPosition = absPosition;

        int centerPosX = width / 2;
        int centerPosY = height / 2;

        if (height % 2 == 0 || width % 2 == 0)
        {
            if (wallType == Wall.WallType.Left || wallType == Wall.WallType.Right)
            {
                if (height % 2 == 0 && gridPosition.y == centerPosY)
                {
                    if (wallType == Wall.WallType.Left)
                        doorPosition += new Vector3(0, 0, -2 * gridSize + (gridSize / 2));
                    else if (wallType == Wall.WallType.Right)
                        doorPosition += new Vector3(0, 0, gridSize / 2);

                    return doorPosition;
                }
                else
                {
                    return doorPosition;
                }
            }
            else if (wallType == Wall.WallType.Top || wallType == Wall.WallType.Bottom)
            {
                if (width % 2 == 0 && gridPosition.x == centerPosX)
                {
                    if (wallType == Wall.WallType.Top)
                        doorPosition += new Vector3(-2 * gridSize + (gridSize / 2), 0, 0);
                    else if (wallType == Wall.WallType.Bottom)
                        doorPosition += new Vector3(gridSize / 2, 0, 0);

                    return doorPosition;
                }
                else
                {
                    return doorPosition;
                }
            }
        }

        return doorPosition;
    }

    public Vector3 GetDoubleDoorOffset(Wall.WallType wallType)
    {
        if (wallType == Wall.WallType.Left)
        {
            return new Vector3(0, 0, -2 * gridSize + (gridSize / 2));
        }
        else if (wallType == Wall.WallType.Right)
        {
            return new Vector3(0, 0, gridSize / 2);
        }
        else if (wallType == Wall.WallType.Top)
        {
            return new Vector3(-2 * gridSize + (gridSize / 2), 0, 0);
        }
        else if (wallType == Wall.WallType.Bottom)
        {
            return new Vector3(gridSize / 2, 0, 0);
        }

        return Vector3.zero;
    }

    public static Wall.WallType? GetWallType(RoomConfig roomConfig, Vector2Int gridPosition)
    {
        if (gridPosition.x == -1 && gridPosition.y >= 0 && gridPosition.y < roomConfig.height)
            return Wall.WallType.Left;

        if (gridPosition.x == roomConfig.width && gridPosition.y >= 0 && gridPosition.y < roomConfig.height)
            return Wall.WallType.Right;

        if (gridPosition.y == -1 && gridPosition.x >= 0 && gridPosition.x < roomConfig.width)
            return Wall.WallType.Bottom;

        if (gridPosition.y == roomConfig.height && gridPosition.x >= 0 && gridPosition.x < roomConfig.width)
            return Wall.WallType.Top;

        return null;
    }

    public static bool HasDoorOnSide(RoomConfig roomConfig, Wall.WallType wallType)
    {
        for (int i = 0; i < roomConfig.doorConfigs.Count; i++)
        {
            if (roomConfig.doorConfigs[i].wallType == wallType)
                return true;
        }

        return false;
    }

    public static bool IsDoor(RoomConfig roomConfig, Vector2Int gridPosition, Wall.WallType wallType)
    {
        int centerPosX = roomConfig.width / 2;
        int centerPosY = roomConfig.height / 2;

        bool hasDoor = HasDoorOnSide(roomConfig, wallType);

        if (gridPosition.y >= 0 && gridPosition.y < roomConfig.height)
        {
            if (gridPosition.x == -1 && wallType == Wall.WallType.Left)
                return (hasDoor && (gridPosition.y == centerPosY || (roomConfig.height % 2 == 0 && gridPosition.y == centerPosY - 1)));

            if (gridPosition.x == roomConfig.width && wallType == Wall.WallType.Right)
                return (hasDoor && (gridPosition.y == centerPosY || (roomConfig.height % 2 == 0 && gridPosition.y == centerPosY - 1)));
        }

        if (gridPosition.x >= 0 && gridPosition.x < roomConfig.width)
        {
            if (gridPosition.y == -1 && wallType == Wall.WallType.Bottom)
                return (hasDoor && (gridPosition.x == centerPosX || (roomConfig.width % 2 == 0 && gridPosition.x == centerPosX - 1)));

            if (gridPosition.y == roomConfig.height && wallType == Wall.WallType.Top)
                return (hasDoor && (gridPosition.x == centerPosX || (roomConfig.width % 2 == 0 && gridPosition.x == centerPosX - 1)));
        }

        return false;
    }

    public static bool ShouldSpawnDoubleDoor(Room roomConfig, Wall.WallType wallType, Vector2Int gridPosition)
    {
        int centerPosX = roomConfig.width / 2;
        int centerPosY = roomConfig.height / 2;

        if (roomConfig.height % 2 == 0 || roomConfig.width % 2 == 0)
        {
            if ((wallType == Wall.WallType.Left || wallType == Wall.WallType.Right) && gridPosition.y == centerPosY)
            {
                return true;
            }
            else if ((wallType == Wall.WallType.Top || wallType == Wall.WallType.Bottom) && gridPosition.x == centerPosX)
            {
                return true;
            }
        }

        return false;
    }
}
