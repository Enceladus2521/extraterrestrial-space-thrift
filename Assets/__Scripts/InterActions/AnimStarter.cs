using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimStarter : MonoBehaviour
{
    public void StartAnim()
    {
        GetComponent<Animation>().Play();
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        GetComponent<Animation>().playAutomatically = false;
    }
   
}
