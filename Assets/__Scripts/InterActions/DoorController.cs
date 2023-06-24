using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animation))]
public class DoorController : MonoBehaviour
{

    [SerializeField] private bool isOpen = false;
    public bool isLocked = false;

     public float autoCloseTime;
 
    [SerializeField] private AnimationClip openAnimation;
    [SerializeField] private AnimationClip closeAnimation;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        GetComponent<Animation>().playAutomatically = false;    
    }
    public void OpenDoor()
    {
        
        if (isLocked)
        {
            Debug.Log("Door is locked");
            return;
        }
        isOpen = true;
       Debug.Log("Open Door");
        

        //play open animation
        GetComponent<Animation>().clip = openAnimation;
        GetComponent<Animation>().Play();

        StartCoroutine(AutoClose());
    }


    public IEnumerator AutoClose()
    {
        if(autoCloseTime == 0) yield break;


        yield return new WaitForSeconds(autoCloseTime);

        CloseDoor();
    }

    public void CloseDoor()
    {        
        isOpen = false;
        Debug.Log("Close Door");

        //play close animation
        GetComponent<Animation>().clip = closeAnimation;
        GetComponent<Animation>().Play();
        
    }
}
