using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSpawnManager : MonoBehaviour
{
    
    
    public void OnPlayerConnected()
    {
        //Deactivate Main Camera
        Camera.main.gameObject.SetActive(false);
    }

    public void OnPlayerDisconnected()
    {
        
    }
   
    
}
