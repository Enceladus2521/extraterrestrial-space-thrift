using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class RoomInternal : ScriptableObject
{
    public List<GameObject> wallTypes;
    public List<GameObject> floorTypes;
    public List<GameObject> singleDoorTypes;
    public List<GameObject> doubleDoorTypes;
    public List<GameObject> interactableTypes;
    public List<GameObject> entityTypes;

    public List<GameObject> heavyObjects;

}