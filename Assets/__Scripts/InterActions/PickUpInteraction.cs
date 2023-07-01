using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Interacter))]
[RequireComponent(typeof(Rigidbody))]
public class PickUpInteraction : MonoBehaviour
{
    public int ammoAmount = 0;
    public int healthAmount = 0;
    public int amorAmount = 0;
    public int moneyAmount = 0;

    [SerializeField] private float flySpeed = 6f;

    [SerializeField] private float pickupRange = 1.5f;

    [SerializeField] private bool InstantPickup = false;


    
    private void Start()
    {
        //get interacter
        Interacter interacter = GetComponent<Interacter>();
        //clear all events
        interacter.events.RemoveAllListeners();
        //add loot function to events
        interacter.events.AddListener(Loot);
        
    }

    public void SetAmount(int ammo, int health, int amor, int money, float pickupRange)
    {
        ammoAmount = ammo;
        healthAmount = health;
        amorAmount = amor;
        moneyAmount = money;
        this.pickupRange = pickupRange;
    }

    public void Loot()
    {
        if (!InstantPickup)
        {
            StartCoroutine(Pickup());
        }
        else
        {
            InstantiatePickup();
        }
       
    }

    private void InstantiatePickup()
    {
        //get closest player
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

        
        if (ammoAmount > 0)
        {
            closestPlayer.GetComponent<PlayerStats>().AddAmmo(ammoAmount);
        }
        
        if (healthAmount > 0)
        {
            closestPlayer.GetComponent<PlayerStats>().AddHealth(healthAmount);
        }
        
        if (amorAmount > 0)
        {
            closestPlayer.GetComponent<PlayerStats>().AddArmor(amorAmount);
        }

        if (moneyAmount > 0)
        {
            closestPlayer.GetComponent<PlayerStats>().AddMoney(moneyAmount);
        }
    }


    IEnumerator Pickup()
    {

        
        //get player
        GameObject player = GetComponent<Interacter>().Player;

        //add rb if not already there
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }

        //fly to player rb
        Rigidbody rb = GetComponent<Rigidbody>();
        while (Vector3.Distance(transform.position, new Vector3(player.transform.position.x, 1f, player.transform.position.z)) > pickupRange)
        {
            rb.velocity = (new Vector3(player.transform.position.x, 1f, player.transform.position.z) - transform.position).normalized * flySpeed;
            yield return null;
        }
        


        
        if (ammoAmount > 0)
        {
            player.GetComponent<PlayerStats>().AddAmmo(ammoAmount);
        }

        
        if (healthAmount > 0)
        {
            player.GetComponent<PlayerStats>().AddHealth(healthAmount);
        }

        
        if (amorAmount > 0)
        {
            player.GetComponent<PlayerStats>().AddArmor(amorAmount);
        }

        
        if (moneyAmount > 0)
        {
            player.GetComponent<PlayerStats>().AddMoney(moneyAmount);
        }

        Destroy(gameObject);

        
    }
}
