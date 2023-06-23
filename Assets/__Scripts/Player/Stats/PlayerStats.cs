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


    [Header("Stats")]
    [SerializeField] private float health;    
    [SerializeField] private float maxHealth;

    
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
        }
    }

    





    


}
