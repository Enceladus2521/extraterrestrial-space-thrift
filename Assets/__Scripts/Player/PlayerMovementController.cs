using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{

    [SerializeField] private float PlayerMovementSpeed = 10f;
    private float curentPlayerMovementSpeed;
    [Range(1, 5)]
    [SerializeField] private float PlayerDashSpeed = 2f;

    [SerializeField] private float PlayerDashDuration = 1f;
    [SerializeField] private float PlayerDashCooldown = 1f;

    private Vector2 moveVector;






    private CharacterController _characterController;

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        curentPlayerMovementSpeed = PlayerMovementSpeed;
    }

    private void LateUpdate()
    {
        if (!_characterController.isGrounded)
        {
            _characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }

    public void ReciveMoveInput(Vector2 inputVector)
    {
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);
        moveVector = movementVector;
        _characterController.Move(movementVector * curentPlayerMovementSpeed * Time.deltaTime);
    }

    public void ReciveDash()
    {
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        //dash in direction of movement
        curentPlayerMovementSpeed = PlayerMovementSpeed * PlayerDashSpeed;
        yield return new WaitForSeconds(PlayerDashDuration);
        curentPlayerMovementSpeed = PlayerMovementSpeed;
        yield return new WaitForSeconds(PlayerDashCooldown);

    }

}
