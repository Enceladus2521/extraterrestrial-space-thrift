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

    

    Rigidbody rb;




    

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        curentPlayerMovementSpeed = PlayerMovementSpeed;
    }

    private void LateUpdate()
    {
        //move player forward in direction of input
        Vector3 movementVector = new Vector3(moveVector.x, 0, moveVector.y);
        transform.Translate(movementVector * curentPlayerMovementSpeed * Time.deltaTime, Space.World);
       


       

        

    }

    public void ReciveMoveInput(Vector2 inputVector)
    {       
        moveVector = inputVector;        
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
