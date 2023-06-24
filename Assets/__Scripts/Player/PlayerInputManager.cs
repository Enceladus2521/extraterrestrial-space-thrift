using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(WeaponController))]
public class PlayerInputManager : MonoBehaviour
{
    PlayerMovementController _playerMovementController;
    PlayerAnimationController _playerAnimationController;

    WeaponController _weaponController;
    

    private void Awake()
    {
        _playerMovementController = GetComponent<PlayerMovementController>();    
        _playerAnimationController = GetComponent<PlayerAnimationController>();   
        _weaponController = GetComponent<WeaponController>(); 
    }

    public void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        _playerMovementController.ReciveMoveInput(inputVector);
    }  
    
    public void OnSprint(InputValue value)
    {
        _playerMovementController.ReciveSprintInput(value.isPressed);
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

    public void OnReload()
    {
        _weaponController.Reload();
    }

    public void OnShootStart()
    {
        //Todo: Start shooting
        //_weaponController.ShootStart();
        
    }

    public void OnShootStop()
    {
        //Todo: Stop shooting
        //_weaponController.ShootEnd();
    }
    


}
