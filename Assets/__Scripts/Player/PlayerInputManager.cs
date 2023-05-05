using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManager : MonoBehaviour
{
    PlayerMovementController _playerMovementController;
    

    private void Awake()
    {
        _playerMovementController = GetComponent<PlayerMovementController>();        
    }

    public void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        _playerMovementController.ReciveMoveInput(inputVector);
    }  

    public void OnDash()
    {   
        _playerMovementController.ReciveDash();
    }   

    public void OnLook(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        _playerMovementController.ReciveLookInput(inputVector);
    } 

    


}
