using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Door
{
    public Wall.WallType wallType;
    public bool isConnected;

    public Vector2Int gridPosition;
    public Vector3 absPosition;

    public static Quaternion GetRotation(Wall.WallType wallType)
    {
        switch (wallType)
        {
            case Wall.WallType.Left:
                return Quaternion.Euler(new Vector3(0, 90, 0));
            case Wall.WallType.Right:
                return Quaternion.Euler(new Vector3(0, -90, 0));
            case Wall.WallType.Top:
                return Quaternion.Euler(new Vector3(0, 180, 0));
            default:
                return Quaternion.identity;
        }
    }

}

