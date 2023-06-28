using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wall
{
       public enum WallType
    {
        Left,
        Right,
        Top,
        Bottom
    }

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

    public static Vector3 GetOffset(Wall.WallType wallType, float gridSize)
    {
        switch (wallType)
        {
            case Wall.WallType.Top:
                return new Vector3(-gridSize / 2f, 0, 0);
            case Wall.WallType.Bottom:
                return new Vector3(gridSize / 2f, 0, 0);
            case Wall.WallType.Left:
                return new Vector3(0, 0, -gridSize / 2f);
            case Wall.WallType.Right:
                return new Vector3(0, 0, gridSize / 2f);
            default:
                return Vector3.zero;
        }
    }

    

}