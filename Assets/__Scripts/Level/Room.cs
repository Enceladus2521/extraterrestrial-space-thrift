using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Room
{
    public RoomInternal internalConfig;

    public float gridSize;
    public Room.RoomType roomType;
    public Vector2 offset;
    public int width;
    public int height;
    public int seed;
    public Transform transform;
    public List<Door> doorConfigs = new List<Door>();

    public bool Connect(Door door) {
        if (door.isConnected)
            return false;
        
        Wall.WallType wallType = door.wallType;
        // get opposite wall type of the door
        Wall.WallType oppositeWallType = wallType == Wall.WallType.Left ? Wall.WallType.Right : Wall.WallType.Left;
        // find own door with opposite wall type
        Door otherDoor = doorConfigs.Find(d => d.wallType == oppositeWallType);
        if (otherDoor == null)
            return false;

        door.isConnected = true;
        otherDoor.isConnected = true;

        door.otherDoor = otherDoor;
        otherDoor.otherDoor = door;

        return true;
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
        Random.InitState(seed);
        return internalConfig.singleDoorTypes[Random.Range(0, internalConfig.singleDoorTypes.Count)];
    }

    public GameObject getDoubleDoorPrefab(Wall.WallType wallType)
    {
        Random.InitState(seed);
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


}
