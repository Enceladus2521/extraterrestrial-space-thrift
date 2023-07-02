using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;



[RequireComponent(typeof(SphereCollider))]
public class Interacter : MonoBehaviour
{

    public bool AutoTrigger = false;
    public bool InteractOnce = false;

    [SerializeField] private int interactCost = 0;
    [SerializeField] private float interactDelay = 0f;

    [HideInInspector] public GameObject Player;

    private bool Interacted = false;
    [SerializeField] private float interactRange = 1f;
    [SerializeField] private Vector3 interactOffset;

    //list of unity events that can be triggered
    public UnityEvent events;

    private GameObject InteractCanvasInstance;
    [SerializeField] private string InteractText = "Press F or X to interact";

    [SerializeField] private Vector3 CanvasOffset = new Vector3(0, 2f, 0);

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
                //disable sphere collider
                GetComponent<SphereCollider>().enabled = false;
                return;
            }
            

            if (interactDelay > 0) StartCoroutine(InteractDelay());
            

            if(interactCost > 0)
            {
                if (other.GetComponent<PlayerStats>().GetMoney() < interactCost)
                {       
                    Debug.Log("Not enough money");             
                    return;
                }
            }

            if (AutoTrigger)
            {
                //find closest player
                GameObject closestPlayer = null;
                float closestDistance = Mathf.Infinity;
                Debug.Log("Heavy load of Player");
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = player;
                    }
                }

                Player = closestPlayer;


                events.Invoke();
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

    IEnumerator InteractDelay()
    {
        yield return new WaitForSeconds(interactDelay);
        Interacted = false;
        //enable sphere collider
        GetComponent<SphereCollider>().enabled = true;

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerInteractManager>().RemoveEvent(this.gameObject);

        if (InteractCanvasInstance != null)
            Destroy(InteractCanvasInstance);
        }
    }


    public void Interact(GameObject player)
    {
        if (Interacted && InteractOnce)
        {
            return;
        }

        Player = player;
        if(interactCost > 0 && player.GetComponent<PlayerStats>()!= null)
        player.GetComponent<PlayerStats>().TakeMoney(interactCost);
        else
        {
            Debug.Log("No player stats");
        }

        //hide interact canvas
        if (InteractCanvasInstance != null)
        Destroy(InteractCanvasInstance);

        //trigger events
        events.Invoke();
        Interacted = true;
    }

    public void SetInteractText(string text)
    {
        InteractText = text;
    }

    public void SetHasInteracted(bool value)
    {
        Interacted = value;
    }
   


    private void OnDestroy()
    {
        if (InteractCanvasInstance != null)
        Destroy(InteractCanvasInstance);
    }


    private void OnEnable()
    {
        //enable sphere collider
        GetComponent<SphereCollider>().enabled = true;
    }

    
    private void OnDisable()
    {
        //disable sphere collider
        GetComponent<SphereCollider>().enabled = false;

        if(InteractCanvasInstance != null)
        {
            // TODO Player?
            Player?.GetComponent<PlayerInteractManager>().RemoveEvent(this.gameObject);
            if (InteractCanvasInstance != null)
            Destroy(InteractCanvasInstance);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + interactOffset, interactRange);
    }

    
    
}
