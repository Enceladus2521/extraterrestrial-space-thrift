using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> interactables;

    private void OnInteract()
    {
        foreach (GameObject e in interactables)
        {
            e.GetComponent<Interacter>().Interact();
            Debug.Log("Interacted with " + e.name);
        }
    }

    public void AddEvent(GameObject e)
    {
        interactables.Add(e);
    }

    public void RemoveEvent(GameObject e)
    {
        interactables.Remove(e);
    }
}
