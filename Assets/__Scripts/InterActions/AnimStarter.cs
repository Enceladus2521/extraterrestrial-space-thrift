using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStarter : MonoBehaviour
{
    public void StartAnim()
    {
        GetComponent<Animation>().Play();
    }
}
