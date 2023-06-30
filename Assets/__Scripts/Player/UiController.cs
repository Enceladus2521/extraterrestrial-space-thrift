using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UiController : MonoBehaviour
{
    //Instance
    public static UiController Instance;

   
    private void Awake()
    {
        //Check if instance is null
        if (Instance == null)
        {
            //Set instance to this
            Instance = this;
        }
        else
        {
            //destroy gameobject
            Destroy(gameObject);
            
        }
    }

    
    private void Start()
    {
        //Deactivate all player stats
        foreach (var playerStat in playerStats)
        {
            playerStat.StatsPanel.SetActive(false);
        }
    }



    [SerializeField] List<UiPlayerStats> playerStats;

    public void DisplayPlayerStats(int playerID, bool display)
    {
        playerStats[playerID].StatsPanel.SetActive(display);
    }

    public void UpdateHealth(int playerID, float maxHealth, float health)
    {
        playerStats[playerID].healthSlider.maxValue = maxHealth;
        playerStats[playerID].healthSlider.value = health;
    }

    public void UpdateArmor(int playerID, float maxArmor, float armor)
    {
        playerStats[playerID].ArmorSlider.maxValue = maxArmor;
        playerStats[playerID].ArmorSlider.value = armor;
    }

    public void UpdateXp(int playerID, float maxXP, float xp, int level)
    {
        playerStats[playerID].xpSlider.maxValue = maxXP;
        playerStats[playerID].xpSlider.value = xp;
        playerStats[playerID].LevelText.text = level.ToString();
    }
    
    public void UpdateAmmo(int playerID, int ammo, int ammoInClip, bool isFullAuto)
    {
        //check if player id is valid
        if (playerID > playerStats.Count && playerID < 0)
        {
            Debug.LogWarning("Player ID is invalid");
            return;
        }

        if (isFullAuto)
        {
            playerStats[playerID].SemiAuto.SetActive(false);
            playerStats[playerID].FullAuto.SetActive(true);
        }
        else
        {
            playerStats[playerID].SemiAuto.SetActive(true);
            playerStats[playerID].FullAuto.SetActive(false);
        }
        
        playerStats[playerID].AmmoText.text = ammoInClip + " / " + ammo;
    }


    public void UpdateMoney(int playerID, int money)
    {
        string moneyText = new string(money.ToString() + " $");
        playerStats[playerID].MoneyText.text = moneyText;
    }
    
    

    
    
}


[System.Serializable]
public class UiPlayerStats
{
    public GameObject StatsPanel;

    [Header("Player Health")]
    public Slider healthSlider;
    
    [Header("Player Armor")]
    public Slider ArmorSlider;
    
    [Header("Player XP")]
    public Slider xpSlider;
    public TMP_Text LevelText;
    
    [Header("Player Ammo")]
    public TMP_Text AmmoText;
    public GameObject SemiAuto;
    public GameObject FullAuto;

    [Header("Player Money")]
    public TMP_Text MoneyText;
}