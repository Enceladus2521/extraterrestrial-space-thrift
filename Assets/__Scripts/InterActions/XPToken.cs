using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class XPToken : MonoBehaviour
{
   
    public float amount = 10f;
    [SerializeField] private float maxAmount = 10000f;

    [SerializeField] private float flySpeed = 15f;

    [SerializeField] private Gradient colorGradient;

    
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        //set color based on amount
        GetComponent<MeshRenderer>().material.color = colorGradient.Evaluate(amount / maxAmount);
    }

    public void SetAmount(float amount)
    {
        this.amount = amount;
        //set color based on amount
        GetComponent<MeshRenderer>().material.color = colorGradient.Evaluate(amount / maxAmount);
        StopCoroutine(FlyToClosestPlayer());
        StartCoroutine(FlyToClosestPlayer());
    }

    public float GetAmount()
    {
        return amount;
    }

    public void FlyToPlayer()
    {
        StopCoroutine(FlyToClosestPlayer());
        StartCoroutine(FlyToClosestPlayer());
    }
    IEnumerator FlyToClosestPlayer()
    {
        
        //get closest player
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        List<GameObject> players = GameManager.Instance?.Players;
        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        //fly to player
        while (Vector3.Distance(transform.position, new Vector3(closestPlayer.transform.position.x,1f, closestPlayer.transform.position.z)) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(closestPlayer.transform.position.x,1f, closestPlayer.transform.position.z), flySpeed * Time.deltaTime);
            yield return null;
        }

        //add xp to player
        closestPlayer.GetComponent<PlayerStats>().AddExperience(amount);

        Destroy(gameObject);
    }

    

    
    
}
