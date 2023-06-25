using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IndividualWeaponData
{
    public int level = 0;
    public int ammoInClip = 0;
    public Rarity rarity;
    public enum Rarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,
        Mythic = 5
    }

    public IndividualWeaponData(int level, int ammoInClip, int rarity)
    {
        this.level = level;
        this.ammoInClip = ammoInClip;
        this.rarity = (Rarity)rarity;
    }
    
    

}
