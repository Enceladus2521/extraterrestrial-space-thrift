using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerStats : MonoBehaviour
{

    [Header("Leveling")]
    [SerializeField] private int level = 0;
    [SerializeField] private float experience = 0;
    private float requiredExperience = 100;

    [SerializeField] private float experienceMultiplier = 1.5f;


    [Header("Stats-------")]
    [Header("Health")]

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    [SerializeField] private int baseHealth = 100;
    [SerializeField] private float healthMultiplier = 1.9f;


    [Header("Amor")]
    [SerializeField] private int armor;
    [SerializeField] private int maxArmor;

    [SerializeField] private int baseArmor = 100;
    [SerializeField] private float armorMultiplier = 1.9f;

  


    [Header("Ammo")]
    [SerializeField] private int ammo = 0;
    [SerializeField] private int maxAmmo;

    [SerializeField] private int baseAmmo = 100;
    [SerializeField] private float ammoMultiplier = 1.1f;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
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
    }

    private void CalculateRequiredExperience()
    {
        requiredExperience = requiredExperience * experienceMultiplier * level;
    }



    public void AddExperience(float amount)
    {
        experience += amount;
        if (experience >= requiredExperience)
        {
            experience -= requiredExperience * experienceMultiplier * level;
            level++;
            CalculateRequiredExperience();
            UpdateStats();

        }
    }

    [EButton("Update Stats")]
    public void UpdateStats()
    {
        maxHealth = (int)(baseHealth * Mathf.Pow(healthMultiplier, level));
        maxArmor = (int)(baseArmor * Mathf.Pow(armorMultiplier, level));        
        maxAmmo = (int)(baseAmmo * Mathf.Pow(ammoMultiplier, level));
        CalculateRequiredExperience();

    }

    public void TakeDamage(int damage)
    {
        int totalDamage = damage;
        if (armor < totalDamage)
        {
            totalDamage -= armor;
            armor = 0;
            health -= totalDamage;
        }
        else
        {
            armor -= totalDamage;
        }

        if (health <= 0)
        {
            Die();
        }
       
    }

    public void Die()
    {
        Debug.Log("Player died");
        //TODO: respawn
        Debug.Log("Respawn");

    }

    public void AddHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void AddArmor(int amount)
    {
        armor += amount;
        if (armor > maxArmor)
        {
            armor = maxArmor;
        }
    }

    

    public void AddAmmo(int amount)
    {
        ammo += amount;
        if (ammo > maxAmmo)
        {
            ammo = maxAmmo;
        }
    }

    public void TakeAmmo(int amount)
    {
        ammo -= amount;
        if (ammo < 0)
        {
            ammo = 0;
        }
    }

    public int GetAmmo()
    {
        return ammo;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetAmor()
    {
        return armor;
    }

    public float GetMaxAmor()
    {
        return maxArmor;
    }













}
