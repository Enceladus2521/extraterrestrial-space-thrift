using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamageDealer : MonoBehaviour
{
    
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {            
        //if other is has rigidbody get weapon in parent
        if(other.GetComponent<Rigidbody>() != null)
        {
            //get weapon
            if(transform.parent.GetComponent<Weapon>() != null)
            {
                transform.parent.GetComponent<Weapon>().OnSliceHit(other.gameObject);
                
            }
        }
        
    }
}
