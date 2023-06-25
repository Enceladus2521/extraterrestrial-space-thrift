using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interacter))]
[RequireComponent(typeof(Rigidbody))]

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponObj weaponObj;
    public IndividualWeaponData individualWeaponData;


    
    private void OnValidate()
    {
        if (weaponObj != null)
        {
            Interacter interacter = GetComponent<Interacter>();
            interacter.AutoTrigger = false;
            interacter.InteractOnce = false;
            interacter.events.AddListener(Pickup);

            interacter.SetInteractText("Press F or X to pickup " + weaponObj.name);            
        }
    }

   
    private void OnEnable()
    {
        if (weaponObj != null)
        {
            Interacter interacter = GetComponent<Interacter>();
            interacter.events.AddListener(Pickup);

            interacter.SetInteractText("Press F or X to pickup " + weaponObj.name);

            individualWeaponData = new IndividualWeaponData(0,0,0);
        }
    }

    public void Pickup()
    {
        //get player
        GameObject player = GetComponent<Interacter>().Player;

        //add to player
        if (player.GetComponent<WeaponController>().PickUpWeapon(weaponObj, individualWeaponData))
        {
            //remove from world
            Destroy(gameObject);
        }
    }

    public void SetIndividualWeaponData(IndividualWeaponData data)
    {
        individualWeaponData = data;
    }



}
