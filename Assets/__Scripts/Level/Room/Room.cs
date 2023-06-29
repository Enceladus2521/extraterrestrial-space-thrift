using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class Room : ScriptableObject
{
    public RoomInternal internalConfig;
    public float gridSize;
    public RoomConfig.RoomType roomType;
    public Vector2 offset;
    public int width;
    public int height;
    public int seed;
    public List<Door> doorConfigs;
}