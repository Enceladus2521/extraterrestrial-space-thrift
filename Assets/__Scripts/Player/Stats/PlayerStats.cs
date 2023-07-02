using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// This script handles the player stats
/// level, experience, health, armor, ammo,  money
/// </summary>


public class PlayerStats : MonoBehaviour
{


    [Header("Leveling")]
    [SerializeField] private int level = 0;
    [SerializeField] private float experience = 0;
    private float currentRequiredExperience = 0;
    private float baseRequiredExperience = 100;

    [SerializeField] private float experienceMultiplier = 1.5f;


    [Header("Stats-------")]
    [Header("Health")]

    [SerializeField] private long health;    
    [SerializeField] private long maxHealth;
    
    


    [Header("Amor")]
    [SerializeField] private long armor;
    [SerializeField] private long maxArmor;
    
   




    [Header("Ammo")]
    [SerializeField] private int ammo = 100;
    [SerializeField] private int maxAmmo = 100;    
    [SerializeField] private float ammoMultiplier = 1.2f;


    

    [Header("Money")]
    [SerializeField] private int money = 0;


    [SerializeField] Material[] materials;
    [SerializeField] GameObject playerHighlight;

    




    
    private void Start()
    {
        

        CalculateRequiredExperience();
        
        SetStats();
        UpdateStats();

    }

    /// <summary>
    /// This function is called in the start function to set initial required experience and stats
    /// </summary>
    private void SetStats()
    {
        //set stats
        health = maxHealth;
        armor = maxArmor;
        ammo = maxAmmo;
    
        //set UI
        UiController.Instance.DisplayPlayerStats(GetComponent<PlayerInput>().playerIndex, true);
        UiController.Instance.UpdateHealth(GetComponent<PlayerInput>().playerIndex, maxHealth, health);
        UiController.Instance.UpdateArmor(GetComponent<PlayerInput>().playerIndex, maxArmor, armor);
        UiController.Instance.UpdateXp(GetComponent<PlayerInput>().playerIndex, currentRequiredExperience, experience, level);
        UiController.Instance.UpdateMoney(GetComponent<PlayerInput>().playerIndex, money);
        UiController.Instance.UpdateAmmo(GetComponent<PlayerInput>().playerIndex, ammo,0, false);

        //Set player color corresponding to player index check if player highlight is not null and check if materials is not null
        if (playerHighlight != null && materials[GetComponent<PlayerInput>().playerIndex] != null)
        {
            playerHighlight.GetComponent<MeshRenderer>().material = materials[GetComponent<PlayerInput>().playerIndex];
        }

    }








    /// <summary>
    /// This function is called when the player levels up
    /// </summary>
    public void UpdateStats()
    {
        maxHealth = 100;
        maxArmor = 100;
        maxAmmo = (int) (100 * (ammoMultiplier * (level + 1))); //ammoMultiplier * (level + 1) is the formula for ammo
        CalculateRequiredExperience();
        UiController.Instance.UpdateXp(GetComponent<PlayerInput>().playerIndex, currentRequiredExperience, experience, level); //update xp ui
    }



    public void Die()
    {
        Debug.Log("Player died");
        GameManager.Instance?.OnPlayerDied(gameObject);
        gameObject.SetActive(false);

    }

    //Health------------------------
    /// <summary>
    /// This function is called when the player Picks up a health pack
    /// </summary>
    public void AddHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UiController.Instance.UpdateHealth(GetComponent<PlayerInput>().playerIndex, maxHealth, health);
    }
   

    
/// <summary>
/// This function is called when the player takes damage
/// This function checks if the armor is broken or not and then applies the damage
/// </summary>
    public void TakeDamage(float damage)
    {
        float totalDamage = damage;        
        if (armor < totalDamage)
        {
            Debug.Log("Armor broken");
            totalDamage -= armor;
            armor = 0;

            health -= (int) totalDamage;
            
        }
        else
        {
            Debug.Log("Armor not broken");
            armor -= (int) totalDamage;
        }

        if (health <= 0)
        {
            Die(); //die if health is 0
        }

        //update UI
        UiController.Instance.UpdateArmor(GetComponent<PlayerInput>().playerIndex, maxArmor, armor);
        UiController.Instance.UpdateHealth(GetComponent<PlayerInput>().playerIndex, maxHealth, health);

    }
    //Armor-------------------------

    /// <summary>
    /// This function is called when the player picks up an armor pack
    /// </summary>
    public void AddArmor(int amount)
    {
        armor += amount;
        if (armor > maxArmor)
        {
            armor = maxArmor;
        }
        UiController.Instance.UpdateArmor(GetComponent<PlayerInput>().playerIndex, maxArmor, armor);

    }

    


    //Ammo--------------------------

    /// <summary>
    /// This function is called when the player picks up an ammo pack
    /// </summary>
    public void AddAmmo(int amount)
    {
        ammo += amount;
        if (ammo > maxAmmo)
        {
            ammo = maxAmmo;
        }
        UiController.Instance.UpdateAmmo(GetComponent<PlayerInput>().playerIndex, ammo, maxAmmo, true); //update ammo ui
    }

    /// <summary>
    /// This function is called when the player shoots
    /// </summary>
    public void TakeAmmo(int amount)
    {
        ammo -= amount;
        if (ammo < 0)
        {
            ammo = 0;
        }
        UiController.Instance.UpdateAmmo(GetComponent<PlayerInput>().playerIndex, ammo, maxAmmo, true);//update ammo ui

    }


    public int GetAmmo()
    {
        return ammo;
    }

   


    //Level-------------------------
    public float GetLevel()
    {
        return level;

    }

    public float GetExperience()
    {
        return experience;
    }

   
    /// <summary>
    /// This function is called when the player gets experience added to them
    /// </summary>

    public void AddExperience(float amount)
    {
        experience += amount;

        while (experience >= currentRequiredExperience)
        {
            //Debug.Log("Level up");
            experience -= currentRequiredExperience;
            level++;

            

            UpdateStats();
        }
        UiController.Instance.UpdateXp(GetComponent<PlayerInput>().playerIndex, currentRequiredExperience, experience, level); //update xp ui
    }

    private void CalculateRequiredExperience()
    {
        currentRequiredExperience = baseRequiredExperience * experienceMultiplier * level; //formula for required experience
    }


    //Money-------------------------
    public int GetMoney()
    {
        return money;
    }

    /// <summary> 
    /// This function is called when the player picks up money
    /// </summary>
    public void AddMoney(int amount)
    {
        money += amount;
        UiController.Instance.UpdateMoney(GetComponent<PlayerInput>().playerIndex, money);
    }

    /// <summary>
    /// This function is called when the player buys something
    /// </summary>
    public void TakeMoney(int amount)
    {
        money -= amount;
        UiController.Instance.UpdateMoney(GetComponent<PlayerInput>().playerIndex, money);
    }

















}
