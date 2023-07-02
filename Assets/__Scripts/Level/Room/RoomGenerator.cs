using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class RoomGenerator : MonoBehaviour
{
    public RoomConfig roomConfig = new RoomConfig();
    Vector2 absOffset = Vector2.zero;



    public void UpdateConfig(RoomConfig configuration)
    {
        roomConfig = configuration;
    }

    List<Vector2> availablePositions = new List<Vector2>();
    List<Vector2> spawnedPositions = new List<Vector2>();

    public void GenerateRoom()
    {

        absOffset = new Vector2(
            -(roomConfig.width - 2) / 2f,
            -roomConfig.height / 2f
        );



        // create a list of positions with room witdh and higth
        availablePositions.Clear();
        spawnedPositions.Clear();

        // Refill the availablePositions list
        for (int x = 0; x < roomConfig.width; x++)
        {
            for (int y = 0; y < roomConfig.height; y++)
            {
                if (!spawnedPositions.Contains(new Vector2(x, y)))
                    availablePositions.Add(new Vector2(x, y));
                else break;
            }
        }

        Random.InitState(roomConfig.seed);

        GenerateFloor();
        GenerateWalls();
    
        GenerateHeavyObjects();
        GenerateInteractables();
        GenerateEnemies();
    }

    bool IsInCenterBox(Vector2 position, float boxSize)
    {
        return position.x >= -boxSize / 2 && position.x <= boxSize / 2 && position.y >= -boxSize / 2 && position.y <= boxSize / 2;
    }

    // a function to remove the middle 3x3 field from availablePositions
    void RemoveMiddleField()
    {
        for (int i = 0; i < roomConfig.width; i++)
            for (int j = 0; j < roomConfig.height; j++)
                if (IsInCenterBox(new Vector2(i, j), 1.5f))
                    availablePositions.Remove(new Vector2(i, j));
    }

    void GenerateHeavyObjects() {
        // it will place heavy object in center of room

        if(availablePositions.Count == 0) return;
        if(roomConfig.heavyObjects.Count == 0) return;
        RemoveMiddleField();

        GameObject heavyObject = roomConfig.heavyObjects[0];
        GameObject heavyObjectInstance = Instantiate(heavyObject);
        heavyObjectInstance.transform.position = transform.position;
        heavyObjectInstance.transform.parent = transform;

    }


    private Vector3 GetRandomPosition()
    {
 
        int randomIndex = Random.Range(0, availablePositions.Count);
        Vector2 randomPosition = availablePositions[randomIndex];
        availablePositions.RemoveAt(randomIndex);
        spawnedPositions.Add(randomPosition);
        return randomPosition;
    }


    void GenerateEnemies()
    {
        for (int i = 0; i < roomConfig.entityTypes.Count; i++)
        {
            GameObject entityPrefab = roomConfig.entityTypes[i];
            if (entityPrefab != null)
            {
        if (availablePositions.Count == 0) continue;
                Vector3 randomPos = GetRandomPosition() + new Vector3(absOffset.x, 0, absOffset.y);
                randomPos = new Vector3(randomPos.x * roomConfig.gridSize, 2f, randomPos.y * roomConfig.gridSize);
                randomPos += new Vector3(- roomConfig.gridSize / 2f , 0, - (roomConfig.gridSize * (roomConfig.height / 2f)));

                GameObject entityInstance = Instantiate(entityPrefab, randomPos, Quaternion.identity);
                EnemyController entityController = entityInstance.GetComponent<EnemyController>(); // TODO: Make alter to EntityController

                entityController.GenerateRandom(roomConfig.seed, roomConfig.difficulty);


                if (entityController == null)
                {
                    Debug.LogError("EntityController is null");
                    continue;
                }
                
                LevelManager.Instance?.watcher?.entities?.Add(entityController);

                entityInstance.transform.parent = transform;
                entityInstance.name = $"Enemy_{randomPos.x}_{randomPos.z}";
                entityInstance.transform.localPosition = randomPos;
                entityInstance.transform.localRotation = Quaternion.identity;
                entityInstance.transform.localScale = Vector3.one;
            }
        }
    }

    private EntityState GenerateRandomEntityState()
    {
        int difficulty = roomConfig.difficulty;
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
            if (availablePositions.Count == 0) continue;
            Vector3 randomPos = GetRandomPosition() + new Vector3(absOffset.x, 0, absOffset.y);;
            randomPos = new Vector3(randomPos.x * roomConfig.gridSize, 0, randomPos.y * roomConfig.gridSize) + transform.position;
            randomPos += new Vector3(- roomConfig.gridSize / 2 , 0.5f,- (roomConfig.gridSize * (roomConfig.height / 2)) );

            // Add random offset
            float offsetX = Random.Range(-roomConfig.gridSize/3, roomConfig.gridSize/3);
            float offsetZ = Random.Range(-roomConfig.gridSize/3, roomConfig.gridSize/3);
            randomPos += new Vector3(offsetX, 0, offsetZ);

            // Instantiate and place the interactable prefab
            GameObject interactableInstance = Instantiate(interactablePrefab, randomPos, Quaternion.identity);

            // Add random rotation
            float randomRotation = Random.Range(0, 360);
            interactableInstance.transform.rotation = Quaternion.Euler(0, randomRotation, 0);

            interactableInstance.transform.SetParent(transform);
            interactableInstance.name = $"Interactable_{randomPos.x}_{randomPos.z}";
            Interacter interactable = interactableInstance.GetComponent<Interacter>();
            LevelManager.Instance?.watcher?.interactables?.Add(interactable);
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

                if (roomConfig.height > 2)
                {
                    GenerateWall(absPosition + new Vector3(0, roomConfig.gridSize * 1.5f, 0), wallType.Value, gridPosition, mirror: true);
                    GenerateWall(absPosition + new Vector3(0, roomConfig.gridSize * 1.9f, 0), wallType.Value, gridPosition, mirror: false);
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

    void GenerateWall(Vector3 absPosition, Wall.WallType wallType, Vector2Int gridPosition, bool mirror = false)
    {
        GameObject wallPrefab = roomConfig.wallTypes[Random.Range(0, roomConfig.wallTypes.Count)];
        Vector3 wallPosition = absPosition + Wall.GetOffset(wallType, roomConfig.gridSize);
        Quaternion wallRotation = Wall.GetRotation(wallType);

        GameObject wallInstance = Instantiate(wallPrefab, wallPosition, wallRotation);
        wallInstance.transform.SetParent(transform);
        // if mirror flip the wall 
        if (mirror)
        {
            if (wallInstance.GetComponent<BoxCollider>())
                Destroy(wallInstance.GetComponent<BoxCollider>());
            if (wallInstance.GetComponent<MeshCollider>())
                Destroy(wallInstance.GetComponent<MeshCollider>());
            wallInstance.transform.localScale = new Vector3(1, -1, 1);

        }
        wallInstance.name = $"Wall_{gridPosition.x}_{gridPosition.y}";
        if (!mirror)
            if (!wallInstance.GetComponent<BoxCollider>() || !wallInstance.GetComponent<MeshCollider>())
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
        LevelManager.Instance?.watcher?.doors?.Add(doorController);

    }

    void InstantiateGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject instance = Instantiate(prefab, position, rotation);
        instance.transform.SetParent(transform);
    }
}