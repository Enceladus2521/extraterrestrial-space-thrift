using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class RoomGenerator : MonoBehaviour
{
    public RoomConfig roomConfig = new RoomConfig();
    Vector2 absOffset = Vector2.zero;



    public void UpdateConfig(RoomConfig configuration){
        roomConfig = configuration;
    }

    public void GenerateRoom()
    {   
        
        absOffset = new Vector2(
            -(roomConfig.width - 2) / 2f,
            -roomConfig.height / 2f
        );

        Random.InitState(roomConfig.seed);

        GenerateFloor();
        GenerateWalls();
        GenerateCeiling();
        GenerateInteractables();
        GenerateEnemies();
    }

    // Ceiling will be just a box collider
    void GenerateCeiling()
    {
        float width = roomConfig.width * roomConfig.gridSize;
        float height = roomConfig.height * roomConfig.gridSize;
        Vector3 size = new Vector3(width, 1, height);
        Vector3 ceilingOffset = new Vector3(roomConfig.offset.x * roomConfig.gridSize, 3f, roomConfig.offset.y * roomConfig.gridSize);

        GameObject ceiling = new GameObject("Ceiling");
        ceiling.transform.parent = transform;
        ceiling.AddComponent<BoxCollider>();
        ceiling.transform.localScale = size;
        ceiling.transform.position = ceilingOffset;


    }

    void GenerateEnemies()
    {
        for (int i = 0; i < roomConfig.entityTypes.Count; i++)
        {
            GameObject entityPrefab = roomConfig.entityTypes[i];
            if (entityPrefab != null)
            {
                float x = Random.Range(2, roomConfig.width - 2) + absOffset.x;
                float y = Random.Range(2, roomConfig.height - 2) + absOffset.y;
                Vector3 absPosition = new Vector3(x * roomConfig.gridSize, 1, y * roomConfig.gridSize) + transform.position;
      

                GameObject entityInstance = Instantiate(entityPrefab, absPosition, Quaternion.identity);
                EntityController entityController = entityInstance.GetComponent<EntityController>();
                entityController.ApplyRoomEffect(GenerateRandomEntityState());
                LevelManager.Instance.watcher?.entities?.Add(entityController);

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

    private EntityState GenerateRandomEntityState()
    {
        int difficulty = roomConfig.difficultyLevel;
        EntityState entityState = new EntityState();
        entityState.healthStats = new HealthStats();
        entityState.movementStats = new MovementStats();
        entityState.combatStats = new CombatStats();
        entityState.healthStats.GenerateRandom(difficulty, roomConfig.seed);
        entityState.movementStats.GenerateRandom(difficulty, roomConfig.seed);
        entityState.combatStats.GenerateRandom(difficulty, roomConfig.seed);
        return entityState;
    }

    void GenerateInteractables()
    {
        // Spread interactables randomly across the room
        for (int i = 0; i < roomConfig.interactableTypes.Count; i++)
        {
            // Randomly select an interactable prefab
            GameObject interactablePrefab = roomConfig.interactableTypes[i];

            // Randomly calculate the position within the room
            float x = Random.Range(1+1, roomConfig.width - 2) + absOffset.x;
            float y = Random.Range(1+1, roomConfig.height - 2) + absOffset.y;
            Vector3 absPosition = new Vector3(x * roomConfig.gridSize, 0, y * roomConfig.gridSize) + transform.position;

                // Instantiate and place the interactable prefab
                GameObject interactableInstance = Instantiate(interactablePrefab, absPosition, Quaternion.identity);
                interactableInstance.transform.SetParent(transform);
                interactableInstance.name = $"Interactable_{x}_{y}";
        }
    }

    void GenerateWalls()
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

                    GenerateDoor(newDoor);
                }
                else
                {
                    GenerateWall(absPosition, wallType.Value, gridPosition);
                }
            }
        }
    }


    void GenerateFloor()
    {
        Color darkGray = new Color(0.05f, 0.05f, 0.05f);
        // Generate the floor tiles
        for (int x = 0; x < roomConfig.width; x++)
        {
            for (int y = 0; y < roomConfig.height; y++)
            {
                GameObject floorPrefab = roomConfig.floorTypes[Random.Range(0, roomConfig.floorTypes.Count)];
                Vector3 absPosition = new Vector3((x + absOffset.x) * roomConfig.gridSize, 0, (y + absOffset.y) * roomConfig.gridSize);
                absPosition += transform.position;
                Quaternion rotation = Quaternion.identity;

                GameObject floorInstance = Instantiate(floorPrefab, absPosition, rotation);
                floorInstance.transform.SetParent(transform);
                floorInstance.name = $"Floor_{x}_{y}";
                if (!floorInstance.GetComponent<BoxCollider>())
                    floorInstance.AddComponent<BoxCollider>();
            }
        }
    }

    void GenerateWall(Vector3 absPosition, Wall.WallType wallType, Vector2Int gridPosition)
    {
            GameObject wallPrefab = roomConfig.wallTypes[Random.Range(0, roomConfig.wallTypes.Count)];
            Vector3 wallPosition = absPosition + Wall.GetOffset(wallType, roomConfig.gridSize);
            Quaternion wallRotation = Wall.GetRotation(wallType);

            GameObject wallInstance = Instantiate(wallPrefab, wallPosition, wallRotation);
            wallInstance.transform.SetParent(transform);
            wallInstance.name = $"Wall_{gridPosition.x}_{gridPosition.y}";
            if (!wallInstance.GetComponent<BoxCollider>())
                wallInstance.AddComponent<BoxCollider>();

    }

    void GenerateDoor(Door door)
    {
        GameObject doorPrefab = roomConfig.getDoorPrefab(door.wallType, roomConfig.seed);

        int centerPosX = roomConfig.width / 2;
        int centerPosY = roomConfig.height / 2;

        if (roomConfig.height % 2 == 0 && (door.wallType == Wall.WallType.Left || door.wallType == Wall.WallType.Right))
        {
            doorPrefab = roomConfig.getDoubleDoorPrefab(door.wallType, roomConfig.seed - 1);
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
            doorPrefab = roomConfig.getDoubleDoorPrefab(door.wallType, roomConfig.seed + 1);
            if (door.gridPosition.x == (centerPosX))
                if (door.wallType == Wall.WallType.Top)
                    door.absPosition += new Vector3(-2 * roomConfig.gridSize + (roomConfig.gridSize / 2), 0, 0);
                else if (door.wallType == Wall.WallType.Bottom)
                    door.absPosition += new Vector3(roomConfig.gridSize / 2, 0, 0);
                else return;
            else return;
        }

        GameObject doorInstance = Instantiate(doorPrefab);
        doorInstance.transform.position = door.absPosition;
        doorInstance.transform.rotation = Door.GetRotation(door.wallType);
        doorInstance.transform.SetParent(transform);
        doorInstance.name = $"Door_{door.gridPosition.x}_{door.gridPosition.y}";
        DoorController doorController = doorInstance.GetComponent<DoorController>();
        doorController.door = door;
        doorController.isLocked = true;
        LevelManager.Instance.watcher?.doors?.Add(doorController);

    }

    void InstantiateGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(prefab, position, rotation);
        instance.transform.SetParent(transform);
    }
}