using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDespawn : MonoBehaviour
{





    IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }


    public void SetTimeToDespawn(float time)
    {
        StopAllCoroutines();
        StartCoroutine(Despawn(time));

    }

    IEnumerator Despawn(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
