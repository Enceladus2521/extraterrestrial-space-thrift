using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RbCatcher : MonoBehaviour
{
    
    
    /// <summary>
    /// This script is used to catch rigidbodies that fall through the floor
    /// </summary>    
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
