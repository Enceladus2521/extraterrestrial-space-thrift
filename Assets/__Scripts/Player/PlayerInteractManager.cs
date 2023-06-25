using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> interactables;

    private void OnInteract()
    {
        //find closest interactable that is not AutoTrigger
        GameObject closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject interactable in interactables)
        {
            if (!interactable.GetComponent<Interacter>().AutoTrigger)
            {
                float distance = Vector3.Distance(transform.position, interactable.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        //interact with closest interactable
        if (closestInteractable != null)
        {
            closestInteractable.GetComponent<Interacter>().Interact(this.gameObject);
        }

        //remove closest interactable from list
        interactables.Remove(closestInteractable);
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
