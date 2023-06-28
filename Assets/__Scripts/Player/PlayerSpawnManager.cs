using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    public void OnPlayerConnected()
    {
        Debug.Log("Player Connected");
        // Deactivate Main Camera
        if (Camera.main != null)
            Camera.main.gameObject.SetActive(false);

    }

    public void OnPlayerDisconnected()
    {

    }

    
}
