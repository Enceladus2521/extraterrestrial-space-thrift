using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private WeaponObj weapon00;
    [SerializeField] private WeaponObj weapon01;
    [SerializeField] private WeaponObj weapon02;

    public IndividualWeaponData[] weaponData = new IndividualWeaponData[3];

    [SerializeField] private WeaponObj defaultWeapon;

    [Range(0, 3)]
    [SerializeField] private float weaponPickUpCooldown = 0.5f;
    private bool canPickup = true;

    [Range(0, 3)]
    [SerializeField] private float weaponCycleCooldown = 0.2f;
    private bool canCycle = true;

    [Header("Hand")]
    [SerializeField] private Transform hand;

    [Range(0, 10)]
    [SerializeField] int unEquipForce = 3;

    [SerializeField] int currentWeaponNumber = 0;

    [SerializeField] private WeaponObj currentWeapon;

    [SerializeField] private GameObject currentWeaponInstance;
    private Weapon currentWeaponScript;


    private void Start()
    {
        if (defaultWeapon != null)
        {
            PickUpWeapon(defaultWeapon, new IndividualWeaponData(0, 0, 0));
        }
    }

    public void CycleWeapon(int weaponNumber)
    {
        //delay between weapon cycles
        if (canCycle) StartCoroutine(CycleWeaponCooldown());
        else return;

        UnEquipWeapon();

        switch (weaponNumber)
        {
            case 0:
                if (weapon00 != null)
                {
                    currentWeapon = weapon00;
                    EquipWeapon(0);
                }
                currentWeaponNumber = 0;
                break;
            case 1:
                if (weapon01 != null)
                {
                    currentWeapon = weapon01;
                    EquipWeapon(1);
                }
                currentWeaponNumber = 1;
                break;
            case 2:
                if (weapon02 != null)
                {
                    currentWeapon = weapon02;
                    EquipWeapon(2);
                }
                currentWeaponNumber = 2;
                break;
        }
    }

    public void CycleWeaponUp()
    {
        //delay between weapon cycles
        if (!canCycle) return;

        switch (currentWeaponNumber)
        {
            case 0:
                CycleWeapon(1);
                break;
            case 1:
                CycleWeapon(2);
                break;
            case 2:
                CycleWeapon(0);
                break;
        }
    }

    public void CycleWeaponDown()
    {
        //delay between weapon cycles
        if (!canCycle) return;

        switch (currentWeaponNumber)
        {
            case 0:
                CycleWeapon(2);
                break;
            case 1:
                CycleWeapon(0);
                break;
            case 2:
                CycleWeapon(1);
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

    public bool PickUpWeapon(WeaponObj weapon, IndividualWeaponData weaponData2, bool dontDrop = false)
    {
        //start cooldown
        if (canPickup) StartCoroutine(PickUpWeaponCooldown());
        else return false;

        //returns if weapon is null
        if (weapon == null) return false;

        if (currentWeapon != null)
        {
            currentWeaponInstance.GetComponent<Weapon>().SetEquipped(false);
            Destroy(currentWeaponInstance);


            weaponData[currentWeaponNumber] = null;

            //set current weapon to null
            currentWeapon = null;

            currentWeaponInstance = null;

            switch (currentWeaponNumber)
            {
                case 0:
                    weapon00 = null;
                    break;
                case 1:
                    weapon01 = null;
                    break;
                case 2:
                    weapon02 = null;
                    break;
            }
        }
        if (currentWeapon != null && !dontDrop) DropWeapon();


        switch (currentWeaponNumber)
        {
            case 0:
                weapon00 = weapon;
                currentWeapon = weapon00;
                break;
            case 1:
                weapon01 = weapon;
                currentWeapon = weapon01;
                break;
            case 2:
                weapon02 = weapon;
                currentWeapon = weapon02;
                break;
        }
        SaveNewWeaponData(currentWeaponNumber, weaponData2);
        EquipWeapon(currentWeaponNumber);


        return true;

    }

    public void DropWeapon()
    {
        //returns if weapon is null
        if (currentWeapon == null) return;

        currentWeaponInstance.GetComponent<Weapon>().SetEquipped(false);


        GameObject oldWeapon = null;
        oldWeapon = currentWeaponInstance;
        //unparent weapon
        oldWeapon.transform.parent = null;

        //add rigidbody if there is none and force to old weapon
        if (oldWeapon.GetComponent<Rigidbody>() == null)
        {
            oldWeapon.AddComponent<Rigidbody>();
        }
        oldWeapon.GetComponent<Rigidbody>().isKinematic = false;
        oldWeapon.GetComponent<Rigidbody>().AddForce(transform.forward * unEquipForce, ForceMode.Impulse);


        //enable collider on old weapon
        oldWeapon.GetComponent<Collider>().enabled = true;

        //get Meshcollider and set trigger to false
        if (oldWeapon.GetComponent<MeshCollider>() != null)
        {
            oldWeapon.GetComponent<MeshCollider>().isTrigger = false;
        }

        //enable Interacter on old weapon
        if (oldWeapon.GetComponent<Interacter>() != null)
        {
            oldWeapon.GetComponent<Interacter>().enabled = true;
        }

        //set oldweapon weapon data to weapon data list at weapon number
        weaponData[currentWeaponNumber] = new IndividualWeaponData(weaponData[currentWeaponNumber].level, currentWeaponInstance.GetComponent<Weapon>().GetAmmoInClip(), (int)weaponData[currentWeaponNumber].rarity);
        oldWeapon.GetComponent<Weapon>().SetIndividualWeaponData(weaponData[currentWeaponNumber]);

        //add rarity trail to old weapon
        GameObject rarityTrail = null;
        switch ((int)weaponData[currentWeaponNumber].rarity)
        {
            case 0:
                rarityTrail = Instantiate(Resources.Load("autoLoad/PF_ComonTrail")) as GameObject;
                break;
            case 1:
                rarityTrail = Instantiate(Resources.Load("autoLoad/PF_UnComonTrail")) as GameObject;
                break;
            case 2:
                rarityTrail = Instantiate(Resources.Load("autoLoad/PF_RareTrail")) as GameObject;
                break;
            case 3:
                rarityTrail = Instantiate(Resources.Load("autoLoad/PF_EpicTrail")) as GameObject;
                break;
            case 4:
                rarityTrail = Instantiate(Resources.Load("autoLoad/PF_LegendaryTrail")) as GameObject;
                break;
            case 5:
                rarityTrail = Instantiate(Resources.Load("autoLoad/PF_MysticTrail")) as GameObject;
                break;
            default:
                rarityTrail = Instantiate(Resources.Load("autoLoad/PF_ComonTrail")) as GameObject;
                break;
        }

        //set rarity trail as child of old weapon
        rarityTrail.transform.parent = oldWeapon.transform;
        //reset rarity trail position
        rarityTrail.transform.localPosition = Vector3.zero;

        // set weapon data to null
        weaponData[currentWeaponNumber] = null;

        //set current weapon to null
        currentWeapon = null;

        currentWeaponInstance = null;

        switch (currentWeaponNumber)
        {
            case 0:
                weapon00 = null;
                break;
            case 1:
                weapon01 = null;
                break;
            case 2:
                weapon02 = null;
                break;
        }

    }

    public void EquipWeapon(int weaponNumber)
    {
        GameObject weaponPrefab = null;

        switch (weaponNumber)
        {
            case 0:
                weaponPrefab = weapon00.weaponPrefab;
                break;
            case 1:
                weaponPrefab = weapon01.weaponPrefab;
                break;
            case 2:
                weaponPrefab = weapon02.weaponPrefab;
                break;
            default:
                weaponPrefab = null;
                break;
        }

        if (weaponPrefab != null)
        {

            //instantiates weapon prefab as child of hand
            GameObject weapon = Instantiate(weaponPrefab, hand.transform.position, hand.transform.rotation);
            weapon.transform.parent = hand;
            weapon.GetComponent<Weapon>().SetEquipped(true);
            weapon.GetComponent<Weapon>().SetAmmoInClip(weaponData[weaponNumber].ammoInClip, weaponData[weaponNumber].level);


            //sets weapon offset based on weapon offset type
            switch ((int)currentWeapon.weaponOffset)
            {
                case 0:
                    weapon.transform.localPosition = currentWeapon.defaultPositionOffset;
                    weapon.transform.localRotation = Quaternion.Euler(currentWeapon.defaultRotationOffset);
                    weapon.transform.localScale = currentWeapon.defaultScaleOffset;
                    break;

                case 1:
                    weapon.transform.localPosition = currentWeapon.customPositionOffset;
                    weapon.transform.localRotation = Quaternion.Euler(currentWeapon.customRotationOffset);
                    weapon.transform.localScale = currentWeapon.customScaleOffset;
                    break;

            }

            currentWeaponInstance = weapon;

            //disable all collider on weapon not child of weapon
            Collider[] colliders = currentWeaponInstance.GetComponents<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            //if rigidbody on weapon, disable rigidbody
            if (currentWeaponInstance.GetComponent<Rigidbody>() != null)
            {
                currentWeaponInstance.GetComponent<Rigidbody>().isKinematic = true;
            }

            //disable Interacter on weapon
            if (currentWeaponInstance.GetComponent<Interacter>() != null)
            {
                currentWeaponInstance.GetComponent<Interacter>().enabled = false;
            }

            //set player and player
            if (currentWeaponInstance.GetComponent<Weapon>() != null)
            {
                currentWeaponScript = currentWeaponInstance.GetComponent<Weapon>();
                currentWeaponScript.SetPlayer(gameObject);
                currentWeaponScript.LoadWeaponData();
            }
        }


    }

    private void SaveNewWeaponData(int weaponNumber, IndividualWeaponData data)
    {
        //sets weapon data to weapon data list at weapon number
        weaponData[weaponNumber] = data;
    }
    public void UnEquipWeapon()
    {
        if (currentWeaponInstance != null)
        {
            currentWeaponInstance.GetComponent<Weapon>().SetEquipped(false);
            weaponData[currentWeaponNumber] = new IndividualWeaponData(weaponData[currentWeaponNumber].level, currentWeaponInstance.GetComponent<Weapon>().GetAmmoInClip(), (int)weaponData[currentWeaponNumber].rarity);
            Destroy(currentWeaponInstance);
            currentWeapon = null;
        }
    }

    IEnumerator PickUpWeaponCooldown()
    {
        canPickup = false;
        yield return new WaitForSeconds(weaponPickUpCooldown);
        canPickup = true;
    }

    IEnumerator CycleWeaponCooldown()
    {
        canCycle = false;
        yield return new WaitForSeconds(weaponCycleCooldown);
        canCycle = true;
    }

    public void Reload()
    {
        if (currentWeapon != null && currentWeaponScript.CanReload())
        {
            GetComponent<PlayerAnimationController>().ReciveReload();
            StartCoroutine(currentWeaponScript.Reload());
        }
    }

    public void ShootStart()
    {
        if (currentWeaponInstance != null)
        {
            currentWeaponScript.SetShooting(true);
        }
    }

    public void ShootEnd()
    {
        if (currentWeaponInstance != null)
        {
            currentWeaponScript.SetShooting(false);
        }
    }
}
public enum Rarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4,
    Mythic = 5
}