using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Loot Table")]
public class LootTableObj : ScriptableObject
{
    public List<Probabilitys> probabilitys;

    public List<GameObject> RarityTrails;
    
    public List<LootTables> LootTables;
}

[System.Serializable]
public class LootTables
{
    public string name; 

    public List<Loot> loots;
}

[System.Serializable]
public class Loot
{
    public GameObject loot;
    public int probability = 10;

}

[System.Serializable]
public class Probabilitys
{
    public string name;
    public int probability = 10;
   
}