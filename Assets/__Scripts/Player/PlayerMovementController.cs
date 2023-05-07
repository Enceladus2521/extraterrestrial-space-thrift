using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;





[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{

    [SerializeField] private float PlayerMovementSpeed = 10f;
    private float curentPlayerMovementSpeed;



    [SerializeField] bool isDashEnabled = true;
    [Range(1, 5)]
    [SerializeField] private float PlayerDashSpeed = 2f;
    [SerializeField] private float PlayerDashDuration = 1f;
    [SerializeField] private float PlayerDashCooldown = 1f;
    private bool isDashing = false;


    [SerializeField] private float ControllerAimSmoothing = 0.1f;

    private Vector2 moveVector;
    private Vector2 lookVector;

    private Camera myCam;

    public bool isGrounded = false;



    public bool isContoller;

    Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        curentPlayerMovementSpeed = PlayerMovementSpeed;

        GetComponent<PlayerInput>().onControlsChanged += OnDeviceChange;
        isContoller = GetComponent<PlayerInput>().currentControlScheme == "Controller";

        myCam = GetComponent<PlayerInput>().camera;
    }

    private void LateUpdate()
    {
        isGrounded = CheckIsGrounded();
        rb.useGravity = !isGrounded;
        MovePlayer();

        RotatePlayer();

    }

    private void MovePlayer()
    {      

        //move player forward in direction of input
        Vector2 movementVector = new Vector2(moveVector.x, moveVector.y);        
        
        Vector2 moveVelocity = movementVector * curentPlayerMovementSpeed;

        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.y);

        if(isGrounded)
        {
            rb.velocity = new Vector3(moveVelocity.x, 0, moveVelocity.y);
        }
               
    }

    private void RotatePlayer()
    {
        //rotate player in direction of input if isContoller
        if (isContoller)
        {
            Vector3 lookVector3 = new Vector3(lookVector.x, 0, lookVector.y);

            
            if (lookVector3.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookVector3), ControllerAimSmoothing);             
            }

        }
        //rotate player in direction of mouse raycasthit on world y = 0 plane if !isContoller
        else
        {
            Ray ray = myCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                Debug.DrawLine(ray.origin, point, Color.red);
                transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
            }
        }

    }


    private bool CheckIsGrounded()
    {
        //check if player is grounded
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }


    private void OnDeviceChange(PlayerInput pi)
    {
        isContoller = pi.currentControlScheme == "Controller";
    }


    public void ReciveLookInput(Vector2 inputVector)
    {
        lookVector = inputVector;
    }
    public void ReciveMoveInput(Vector2 inputVector)
    {
        moveVector = inputVector;        
    }

    public void ReciveDash()
    {
        if(!isDashing && isDashEnabled)
        StartCoroutine(Dash());
    }

    public void ReciveDeviceChange(bool isController)
    {
        this.isContoller = isController;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        //dash in direction of movement
        curentPlayerMovementSpeed = PlayerMovementSpeed * PlayerDashSpeed;
        yield return new WaitForSeconds(PlayerDashDuration);
        curentPlayerMovementSpeed = PlayerMovementSpeed;
        yield return new WaitForSeconds(PlayerDashCooldown);
        isDashing = false;
    }




}
