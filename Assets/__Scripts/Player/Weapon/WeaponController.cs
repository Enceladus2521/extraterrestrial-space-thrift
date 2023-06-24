using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private WeaponObj weapon00;
    [SerializeField] private WeaponObj weapon01;
    [SerializeField] private WeaponObj weapon02;
    [SerializeField] private WeaponObj defaultWeapon;

    [Header("Hand")]
    [SerializeField] private Transform hand;

    [SerializeField] int unEquipForce = 10;

    [SerializeField] int currentWeaponNumber = 0;

    [SerializeField] private WeaponObj currentWeapon;

    [SerializeField] private GameObject currentWeaponInstance;


    private void Start()
    {
        weapon00 = defaultWeapon;
        currentWeapon = defaultWeapon;
        EquipWeapon(0);
    }

    public void CycleWeapon(int weaponNumber)
    {
        if (weaponNumber > 2 || weaponNumber < 0)
        {
            Debug.LogWarning("Weapon number out of range");
            return;
        }


        //check if weapon is null
        //if not null, equip weapon
        //if null dont equip weapon

        switch (weaponNumber)
        {
            case 0:
                if (weapon00 != null)
                {
                    EquipWeapon(0);
                    currentWeapon = weapon00;
                    currentWeaponNumber = 0;
                }
                break;
            case 1:
                if (weapon01 != null)
                {
                    EquipWeapon(1);
                    currentWeapon = weapon01;
                    currentWeaponNumber = 1;
                }
                break;
            case 2:
                if (weapon02 != null)
                {
                    EquipWeapon(2);
                    currentWeapon = weapon02;
                    currentWeaponNumber = 2;
                }
                break;
        }
    }

    public void CycleWeaponUp()
    {
        //check if weapon is null
        //if not null, equip weapon
        //if null dont equip weapon

        switch (currentWeaponNumber)
        {
            case 0:
                if (weapon01 != null)
                {
                    EquipWeapon(1);
                    currentWeapon = weapon01;
                    currentWeaponNumber = 1;
                }
                break;
            case 1:
                if (weapon02 != null)
                {
                    EquipWeapon(2);
                    currentWeapon = weapon02;
                    currentWeaponNumber = 2;
                }
                break;
            case 2:
                if (weapon00 != null)
                {
                    EquipWeapon(0);
                    currentWeapon = weapon00;
                    currentWeaponNumber = 0;
                }
                break;
        }
    }

    public void CycleWeaponDown()
    {
        //check if weapon is null
        //if not null, equip weapon
        //if null dont equip weapon

        switch (currentWeaponNumber)
        {
            case 0:
                if (weapon02 != null)
                {
                    EquipWeapon(2);
                    currentWeapon = weapon02;
                    currentWeaponNumber = 2;
                }
                break;
            case 1:
                if (weapon00 != null)
                {
                    EquipWeapon(0);
                    currentWeapon = weapon00;
                    currentWeaponNumber = 0;
                }
                break;
            case 2:
                if (weapon01 != null)
                {
                    EquipWeapon(1);
                    currentWeapon = weapon01;
                    currentWeaponNumber = 1;
                }
                break;
        }
    }

    public WeaponObj GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public int GetCurrentWeaponNumber()
    {
        return currentWeaponNumber;
    }

    public void PickUpWeapon(WeaponObj weapon)
    {
        //check if weapon is null
        //if null, set weapon to current weapon and equip
        //if not null, drop current weapon, set weapon to current weapon and equip

        if (weapon00 == null)
        {
            weapon00 = weapon;
            EquipWeapon(0);
            currentWeapon = weapon00;
            currentWeaponNumber = 0;
        }
        else if (weapon01 == null)
        {
            weapon01 = weapon;
            EquipWeapon(1);
            currentWeapon = weapon01;
            currentWeaponNumber = 1;
        }
        else if (weapon02 == null)
        {
            weapon02 = weapon;
            EquipWeapon(2);
            currentWeapon = weapon02;
            currentWeaponNumber = 2;
        }
        else
        {
            DropWeapon(currentWeaponNumber);
            switch (currentWeaponNumber)
            {
                case 0:
                    weapon00 = weapon;
                    EquipWeapon(0);
                    currentWeapon = weapon00;
                    currentWeaponNumber = 0;
                    break;
                case 1:
                    weapon01 = weapon;
                    EquipWeapon(1);
                    currentWeapon = weapon01;
                    currentWeaponNumber = 1;
                    break;
                case 2:
                    weapon02 = weapon;
                    EquipWeapon(2);
                    currentWeapon = weapon02;
                    currentWeaponNumber = 2;
                    break;
            }
        }

    }

    private void DropWeapon(int weaponNumber)
    {
        Destroy(currentWeaponInstance);
        GameObject oldWeapon = Instantiate(weapon00.weaponPrefab, transform.position, transform.rotation);
        oldWeapon.transform.parent = null;

        //add rigidbody and force to old weapon
        oldWeapon.AddComponent<Rigidbody>();
        oldWeapon.GetComponent<Rigidbody>().AddForce(transform.forward * unEquipForce, ForceMode.Impulse);
    }

    public void EquipWeapon(int weaponNumber)
    {
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }

        //instantiates weapon prefab and sets it as child of hand
        //sets weapon as current weapon
        GameObject weapon = Instantiate(currentWeapon.weaponPrefab, hand.transform.position, hand.transform.rotation);
        weapon.transform.parent = hand;
        weapon.transform.localPosition = currentWeapon.weaponPositionOffset;
        weapon.transform.localRotation = Quaternion.Euler(currentWeapon.weaponRotationOffset);
        weapon.transform.localScale = currentWeapon.weaponScaleOffset;

        currentWeaponInstance = weapon;
    }

    public void UnEquipWeapon()
    {
        Destroy(currentWeaponInstance);
        currentWeapon = null;
    }


    public void Reload()
    {
        if (currentWeapon != null)
        {
            //TODO:  reload 
            GetComponent<PlayerAnimationController>().ReciveReload();


        }
    }

}
