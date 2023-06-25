using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpInteraction : MonoBehaviour
{
    public int ammoAmount = 0;
    public int healthAmount = 0;
    public int amorAmount = 0;

    [SerializeField] private float flySpeed = 6f;

    [SerializeField] private float pickupRange = 1f;

    [SerializeField] private bool InstantPickup = false;


    public void SetAmount(int ammo, int health, int amor)
    {
        ammoAmount = ammo;
        healthAmount = health;
        amorAmount = amor;
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
        
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        //Todo: add ammo to player
        if (ammoAmount > 0)
        {
            closestPlayer.GetComponent<PlayerStats>().AddAmmo(ammoAmount);
        }

        //Todo: add health to player
        if (healthAmount > 0)
        {
            closestPlayer.GetComponent<PlayerStats>().AddHealth(healthAmount);
        }

        //Todo: add amor to player
        if (amorAmount > 0)
        {
            closestPlayer.GetComponent<PlayerStats>().AddArmor(amorAmount);
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
        


        //Todo: add ammo to player
        if (ammoAmount > 0)
        {
            player.GetComponent<PlayerStats>().AddAmmo(ammoAmount);
        }

        //Todo: add health to player
        if (healthAmount > 0)
        {
            player.GetComponent<PlayerStats>().AddHealth(healthAmount);
        }

        //Todo: add amor to player
        if (amorAmount > 0)
        {
            player.GetComponent<PlayerStats>().AddArmor(amorAmount);
        }

        Destroy(gameObject);

        
    }
}
