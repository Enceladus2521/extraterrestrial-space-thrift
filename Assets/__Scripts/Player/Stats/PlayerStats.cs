using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


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

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    [SerializeField] private int baseHealth = 100;
    [SerializeField] private float healthMultiplier = 1.1f;


    [Header("Amor")]
    [SerializeField] private int armor;
    [SerializeField] private int maxArmor;

    [SerializeField] private int baseArmor = 100;
    [SerializeField] private float armorMultiplier = 1.1f;




    [Header("Ammo")]
    [SerializeField] private int ammo = 0;
    [SerializeField] private int maxAmmo;

    [SerializeField] private int baseAmmo = 100;
    [SerializeField] private float ammoMultiplier = 1.2f;


    [Header("Damage")]
    [SerializeField] private float damageMultiplier = 1.5f;

    [Header("Money")]
    [SerializeField] private int money = 0;


    [SerializeField] Material[] materials;
    [SerializeField] GameObject playerHighlight;

    




    
    private void Start()
    {
        

        CalculateRequiredExperience();
        UpdateStats();
        SetStats();

    }


    private void SetStats()
    {
        health = maxHealth;
        armor = maxArmor;
        ammo = maxAmmo;
    
        Debug.Log("Set stats");
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







    [EButton("Update Stats")]
    public void UpdateStats()
    {
        maxHealth = (int)(baseHealth * Mathf.Pow(healthMultiplier, level));
        maxArmor = (int)(baseArmor * Mathf.Pow(armorMultiplier, level));
        maxAmmo = (int)(baseAmmo * Mathf.Pow(ammoMultiplier, level));
        CalculateRequiredExperience();
        UiController.Instance.UpdateXp(GetComponent<PlayerInput>().playerIndex, currentRequiredExperience, experience, level);
    }



    public void Die()
    {
        Debug.Log("Player died");
        GameManager.Instance?.OnPlayerDied(gameObject);
        gameObject.SetActive(false);

    }

    //Health------------------------
    public void AddHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UiController.Instance.UpdateHealth(GetComponent<PlayerInput>().playerIndex, maxHealth, health);
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void TakeDamage(float damage)
    {
        float totalDamage = damage;        
        if (armor < totalDamage && armor > 0)
        {
            Debug.Log("Armor broken");
            totalDamage -= armor;
            armor = 0;
            
        }
        else
        {
            Debug.Log("Armor not broken");
            armor -= (int) totalDamage;
        }

        if (health <= 0)
        {
            Die();
        }
        UiController.Instance.UpdateArmor(GetComponent<PlayerInput>().playerIndex, maxArmor, armor);
        UiController.Instance.UpdateHealth(GetComponent<PlayerInput>().playerIndex, maxHealth, health);

    }
    //Armor-------------------------
    public void AddArmor(int amount)
    {
        armor += amount;
        if (armor > maxArmor)
        {
            armor = maxArmor;
        }
        UiController.Instance.UpdateArmor(GetComponent<PlayerInput>().playerIndex, maxArmor, armor);

    }

    public float GetAmor()
    {
        return armor;
    }

    public float GetMaxAmor()
    {
        return maxArmor;
    }


    //Ammo--------------------------

    public void AddAmmo(int amount)
    {
        ammo += amount;
        if (ammo > maxAmmo)
        {
            ammo = maxAmmo;
        }
        UiController.Instance.UpdateAmmo(GetComponent<PlayerInput>().playerIndex, ammo, maxAmmo, true);
    }

    public void TakeAmmo(int amount)
    {
        ammo -= amount;
        if (ammo < 0)
        {
            ammo = 0;
        }
        UiController.Instance.UpdateAmmo(GetComponent<PlayerInput>().playerIndex, ammo, maxAmmo, true);

    }

    public int GetAmmo()
    {
        return ammo;
    }

    public float GetDamage()
    {
        return damageMultiplier;
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

    public float GetRequiredExperience()
    {
        return currentRequiredExperience;
    }

    public void AddExperience(float amount)
    {
        experience += amount;

        while (experience >= currentRequiredExperience)
        {
            //Debug.Log("Level up");
            experience -= currentRequiredExperience;
            level++;

            //regen health and armor
            health = maxHealth;
            armor = maxArmor;

            UpdateStats();
        }
        UiController.Instance.UpdateXp(GetComponent<PlayerInput>().playerIndex, currentRequiredExperience, experience, level);
    }

    private void CalculateRequiredExperience()
    {
        currentRequiredExperience = baseRequiredExperience * experienceMultiplier * level;
    }


    //Money-------------------------
    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UiController.Instance.UpdateMoney(GetComponent<PlayerInput>().playerIndex, money);
    }

    public void TakeMoney(int amount)
    {
        money -= amount;
        UiController.Instance.UpdateMoney(GetComponent<PlayerInput>().playerIndex, money);
    }

















}
