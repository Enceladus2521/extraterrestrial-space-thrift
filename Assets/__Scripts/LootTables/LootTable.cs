using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    public int roomLevel;

    [SerializeField] int minLoot = 1, maxLoot = 5;

    [SerializeField] GameObject LootEmitter;
    [SerializeField] GameObject LootTarget;
    [Range(0, 100)]
    [SerializeField] int SpawnForce = 20;
    [SerializeField] int RandomRotationAmount = 15;




    [SerializeField] LootTableObj lootTableObj;










    public void SpawnLoot(float delay = 1f)
    {
        StartCoroutine(SpawnLootCoroutine(delay));

    }

    IEnumerator SpawnLootCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        int lootAmount = Random.Range(minLoot, maxLoot);

        for (int i = 0; i < lootAmount; i++)
        {
            SpawnLoot();
        }



    }

    private void SpawnLoot()
    {
        if (lootTableObj == null)
        {
            Debug.LogError("No LootTableObj assigned to LootTable.cs", this);
            return;
        }

        if (LootEmitter == null || LootTarget == null)
        {
            Debug.LogError("No LootEmitter or LootTarget assigned to LootTable.cs", this);
            return;
        }

        //get random rarity
        int rarityIndex = ChanceIndexReturn(CreatePropList(lootTableObj.probabilitys));

        //get random loot from rarity
        int lootIndex = ChanceIndexReturn(CreateLootProbList(lootTableObj.LootTables[rarityIndex].loots));

        GameObject loot = lootTableObj.LootTables[rarityIndex].loots[lootIndex].loot;
        GameObject trail = lootTableObj.RarityTrails[rarityIndex];
        GameObject lootObject = Instantiate(loot, LootEmitter.transform.position, Quaternion.identity);
        GameObject trailObject = Instantiate(trail, lootObject.transform.position, Quaternion.identity);
        trailObject.transform.SetParent(lootObject.transform);

        //add Rigidbody
        Rigidbody rb = lootObject.AddComponent<Rigidbody>();

        //push loot in direction of target with random force
        rb.AddForce((LootTarget.transform.position - LootEmitter.transform.position).normalized * Random.Range(SpawnForce / 2, SpawnForce), ForceMode.Impulse);


        //add torque
        rb.AddTorque(new Vector3(Random.Range(-RandomRotationAmount, RandomRotationAmount), Random.Range(-RandomRotationAmount, RandomRotationAmount), Random.Range(-RandomRotationAmount, RandomRotationAmount)));



    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (LootEmitter != null)
        {
            Gizmos.color = Color.red;
            //draw line starting at emitter and going in the the emitters forward direction
            Gizmos.DrawLine(LootEmitter.transform.position, LootEmitter.transform.position + LootEmitter.transform.forward * 1f);
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


