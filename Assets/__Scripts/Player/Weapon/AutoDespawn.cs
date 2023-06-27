using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDespawn : MonoBehaviour
{
    

    IEnumerator Start()
    {
        yield return new WaitForSeconds(20f);
        Destroy(gameObject);
    }
}
