using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomConfig
{
    public RoomGenerator.RoomType roomType;
    public Vector2Int offset;
    public bool hasDoorUp;
    public bool hasDoorDown;
    public bool hasDoorRight;
    public bool hasDoorLeft;
    public int width;
    public int height;
}

public class LevelGenerator : MonoBehaviour
{
    public List<RoomConfig> roomConfigs;
    public RoomGenerator roomGeneratorPrefab;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        foreach (RoomConfig config in roomConfigs)
        {
            RoomGenerator roomGenerator = Instantiate(roomGeneratorPrefab);
            roomGenerator.width = config.width;
            roomGenerator.height = config.height;
            roomGenerator.hasDoorUp = config.hasDoorUp;
            roomGenerator.hasDoorDown = config.hasDoorDown;
            roomGenerator.hasDoorRight = config.hasDoorRight;
            roomGenerator.hasDoorLeft = config.hasDoorLeft;
            roomGenerator.offsetX = config.offset.x;
            roomGenerator.offsetY = config.offset.y;
            roomGenerator.Generate();
        }
    }
}
