using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interacter))]
[RequireComponent(typeof(Animator))]
public class MysteryChest : MonoBehaviour
{

    public int roomLevel;
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] LootTableObj lootTableObj;

    private void Start()
    {
        //get interacter
        Interacter interacter = GetComponent<Interacter>();
        //clear all events
        interacter.events.RemoveAllListeners();
        //add loot function to events
        interacter.events.AddListener(OpenChest);
    }


    public void OpenChest()
    {
        GetComponent<Animator>().SetTrigger("open");

        //spawn loot
        SpawnLoot();
    }




    private void SpawnLoot()
    {
        if (lootTableObj == null)
        {
            Debug.LogError("No LootTableObj assigned to LootTable.cs", this);
            return;
        }

        if (SpawnPoint == null)
        {
            Debug.LogError("No SpawnPoint assigned to LootTable.cs", this);
            return;
        }


        //get random rarity
        int rarityIndex = ChanceIndexReturn(CreatePropList(lootTableObj.probabilitys));

        //get random loot from rarity
        int lootIndex = ChanceIndexReturn(CreateLootProbList(lootTableObj.LootTables[rarityIndex].loots));

        GameObject loot = lootTableObj.LootTables[rarityIndex].loots[lootIndex].loot;
        GameObject trail = lootTableObj.RarityTrails[rarityIndex];
        GameObject lootObject = Instantiate(loot, SpawnPoint.transform.position, Quaternion.identity);

        lootObject.transform.SetParent(SpawnPoint.transform);

        lootObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        lootObject.transform.localPosition = new Vector3(0, 0, 0);
        

        GameObject trailObject = Instantiate(trail, lootObject.transform.position, Quaternion.identity);
        trailObject.transform.SetParent(lootObject.transform);
        

        //add Rigidbodyif not already present
        Rigidbody rb = null;
        if (lootObject.GetComponent<Rigidbody>() == null)
        {
            rb = lootObject.AddComponent<Rigidbody>();
        }
        else
        {
            rb = lootObject.GetComponent<Rigidbody>();
        }

        //check if Loot Has Weapon Component
        if (lootObject.GetComponent<Weapon>() != null)
        {
            //set weapon level and rarity 
            lootObject.GetComponent<Weapon>().SetIndividualWeaponData(new IndividualWeaponData(roomLevel, 0, rarityIndex));
            lootObject.GetComponent<Weapon>().dontDrop = true;
        }

        rb.isKinematic = true;
        rb.useGravity = false;

        //start coroutine to check if loot has been looted
        StartCoroutine(CheckIfLooted());

    }

    public void CloseChest()
    {
        GetComponent<Animator>().SetTrigger("close");
        //destroy loot
        DestroyLoot();

        StopAllCoroutines();

        //get interacter
        Interacter interacter = GetComponent<Interacter>();
        interacter.SetHasInteracted(false);
    }
    private void DestroyLoot()
    {
        if (SpawnPoint.transform.childCount > 0)
        {
            Destroy(SpawnPoint.transform.GetChild(0).gameObject);
        }
    }


    IEnumerator CheckIfLooted()
    {
        while (true)
        {
            if (SpawnPoint.transform.childCount > 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                CloseChest();
                yield break;
            }
        }
    }
    public static List<int> CreatePropList(List<Probabilitys> probabilitys)
    {
        List<int> propList = new List<int>();

        for (int i = 0; i < probabilitys.Count; i++)
        {
            propList.Add(probabilitys[i].probability);
        }

        return propList;
    }

    public static List<int> CreateLootProbList(List<Loot> lootTables)
    {
        List<int> propList = new List<int>();

        for (int i = 0; i < lootTables.Count; i++)
        {
            propList.Add(lootTables[i].probability);
        }

        return propList;
    }

    public static int ChanceIndexReturn(List<int> probList)
    {
        /*---------------------------------------------------------------
            Takes in a list of probabilitys and retruns an random index based on that probability

        [50, 20, 40, 100, 200]
        return 0 has a chance of 12.20%
        return 1 has a chance of  4.88%
        return 2 has a chance of  9.76%
        return 3 has a chance of 24.39%
        return 4 has a chance of 48,78%

        chance = probability(200) / totalProbAmount;        
        ---------------------------------------------------------------*/

        bool debug = false;

        int totalProbAmount = 0; //initialising totalpobAmount
        Vector2[] rangeArray = new Vector2[probList.Count]; //initialising rangeArray

        for (int i = 0; i < probList.Count; i++)
        {
            totalProbAmount += probList[i]; //adding the total amount of probabilitys           

            if (i == 0) // if first range
            {
                rangeArray[i] = new(0, probList[i]); //sets first pos of array to 0 to probability
            }
            else // else there has been a range before
            {
                int fromValue = (int)rangeArray[i - 1].y + 1; // calc first value = last range second value + 1
                int ToValue = fromValue + probList[i]; // calc second value = firstValue + probability

                rangeArray[i] = new(fromValue, ToValue); // sets firstVal and secondVal in array
            }
        }
        if (debug) Debug.Log($"TotalProbAmount = {totalProbAmount}");

        int randomValue = Random.Range(0, totalProbAmount); // generates random val
        if (debug) Debug.Log($"randomValue = {randomValue}");


        for (int i = 0; i < rangeArray.Length; i++) //check every range
        {
            if ((rangeArray[i].x <= randomValue) && (rangeArray[i].y >= randomValue)) return i;   // if value is in range     
        }

        return 0;

    }

    public void SetRoomLevel(int level)
    {
        roomLevel = level;
    }


}
