using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;



[RequireComponent(typeof(SphereCollider))]
public class Interacter : MonoBehaviour
{

    [SerializeField] private bool AutoTrigger = false;

    

    [SerializeField] private bool InteractOnce = false;

    private bool Interacted = false;
    [SerializeField] private float interactRange = 1f;
    [SerializeField] private Vector3 interactOffset;

    //list of unity events that can be triggered
    [SerializeField] private List<UnityEvent> events;
    
    private GameObject InteractCanvasInstance;
    [SerializeField] private string InteractText = "Press F or X to interact";

    [SerializeField] private Vector3 CanvasOffset;

    private void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        GetComponent<SphereCollider>().radius = interactRange;
        GetComponent<SphereCollider>().center = interactOffset;
    }



    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("Player"))
        {

            if (Interacted && InteractOnce)
            {
                return;
            }
            


            if (AutoTrigger)
            {
                foreach (UnityEvent e in events)
                {
                    e.Invoke();
                }
                Interacted = true;
                return;
            }

            other.GetComponent<PlayerInteractManager>().AddEvent(this.gameObject);


            if (InteractCanvasInstance != null)
            {
                Destroy(InteractCanvasInstance);
            }

            GameObject variableForPrefab = Resources.Load("autoLoad/PF_InteractCanvas") as GameObject;
            InteractCanvasInstance = Instantiate(variableForPrefab, transform.position + CanvasOffset, Quaternion.identity);
            InteractCanvasInstance.transform.SetParent(transform);
            InteractCanvasInstance.transform.GetChild(0).GetComponent<TMP_Text>().text = InteractText;

        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerInteractManager>().RemoveEvent(this.gameObject);

            Destroy(InteractCanvasInstance);
        }
    }


    public void Interact()
    {
        if (Interacted && InteractOnce)
        {
            return;
        }
        foreach (UnityEvent e in events)
        {
            e.Invoke();
        }
        Interacted = true;
    }

    
    private void OnDestroy()
    {
        Destroy(InteractCanvasInstance);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + interactOffset, interactRange);
    }
}
