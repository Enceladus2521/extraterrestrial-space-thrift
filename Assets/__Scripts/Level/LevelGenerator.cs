using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<RoomConfig> roomConfigs;
    private List<RoomConfig> previousRoomConfigs;
    public List<Transform> roomTransforms;
    [SerializeField]
    private RoomGenerator roomGeneratorPrefab;
    void Start()
    {
        previousRoomConfigs = DeepCopyRoomConfigs(roomConfigs);
        Generate();
    }

    IEnumerator ClearChildrenCoroutine(Transform parent)
    {
        roomTransforms.Clear();
        List<GameObject> objectsToDestroy = new List<GameObject>();
        foreach (Transform child in parent)
        {
            objectsToDestroy.Add(child.gameObject);
        }

        yield return null;

        foreach (GameObject obj in objectsToDestroy)
        {
            if (Application.isPlaying)
                GameObject.Destroy(obj);
            else
                GameObject.DestroyImmediate(obj);
        }
    }

    public void GenerateRooms(List<RoomConfig> configs)
    {
        if (AreRoomConfigsEqual(previousRoomConfigs, configs))
            return;
        roomConfigs = configs;
        Generate();
        previousRoomConfigs = DeepCopyRoomConfigs(roomConfigs);


    }

    public void Generate(bool gizmos = false)
    {
        StartCoroutine(ClearChildrenCoroutine(transform));
        GenerateLevel(gizmos);
    }

    void GenerateLevel(bool gizmos = false)
    {
        for (int i = 0; i < roomConfigs.Count; i++)
        {
            RoomConfig config = roomConfigs[i];
            RoomGenerator roomGeneratorInstance;

            if (i < roomTransforms.Count)
            {
                roomGeneratorInstance = roomTransforms[i].GetComponent<RoomGenerator>();
            }
            else
            {
                GameObject newRoomObject = Instantiate(roomGeneratorPrefab.gameObject, transform);
                newRoomObject.name = "RoomGenerator " + i;

                roomGeneratorInstance = newRoomObject.GetComponent<RoomGenerator>();
                roomTransforms.Add(newRoomObject.transform);
            }

            roomGeneratorInstance.width = config.width;
            roomGeneratorInstance.height = config.height;
            roomGeneratorInstance.hasDoorUp = config.hasDoorUp;
            roomGeneratorInstance.hasDoorDown = config.hasDoorDown;
            roomGeneratorInstance.hasDoorRight = config.hasDoorRight;
            roomGeneratorInstance.hasDoorLeft = config.hasDoorLeft;
            roomGeneratorInstance.offsetX = config.offset.x;
            roomGeneratorInstance.offsetY = config.offset.y;

            if (!gizmos)
                roomGeneratorInstance.Generate();
        }
    }

    private List<RoomConfig> DeepCopyRoomConfigs(List<RoomConfig> original)
    {
        List<RoomConfig> copy = new List<RoomConfig>(original.Count);
        foreach (RoomConfig config in original)
        {
            copy.Add(new RoomConfig
            {
                roomType = config.roomType,
                offset = config.offset,
                hasDoorUp = config.hasDoorUp,
                hasDoorDown = config.hasDoorDown,
                hasDoorRight = config.hasDoorRight,
                hasDoorLeft = config.hasDoorLeft,
                width = config.width,
                height = config.height
            });
        }
        return copy;
    }

    private bool AreRoomConfigsEqual(List<RoomConfig> configs1, List<RoomConfig> configs2)
    {
        if (configs1 == null || configs2 == null)
        {
            return configs1 == configs2;
        }

        if (configs1.Count != configs2.Count)
            return false;

        for (int i = 0; i < configs1.Count; i++)
        {
            if (configs1[i].roomType != configs2[i].roomType ||
                configs1[i].offset != configs2[i].offset ||
                configs1[i].hasDoorUp != configs2[i].hasDoorUp ||
                configs1[i].hasDoorDown != configs2[i].hasDoorDown ||
                configs1[i].hasDoorRight != configs2[i].hasDoorRight ||
                configs1[i].hasDoorLeft != configs2[i].hasDoorLeft ||
                configs1[i].width != configs2[i].width ||
                configs1[i].height != configs2[i].height)
            {
                return false;
            }
        }

        return true;
    }


}
