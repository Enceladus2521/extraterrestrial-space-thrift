using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(Rigidbody))]

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;

    PlayerMovementController playerMovementController;

    [Header("Player Model with Animator")]
    [SerializeField] GameObject playerModel;

    public float XVel;

    public float YVel;
    public float ZVel;

    public float playerRotationOffset = 0f;

    
    void Start()
    {
        
        playerMovementController = GetComponent<PlayerMovementController>();
        StartCoroutine(GetPlayerModel());
    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null) return;

        //get rotation from player movement controller
        playerRotationOffset = transform.rotation.eulerAngles.y;

        //get velocity from character controller
        XVel = GetComponent<PlayerMovementController>().moveVelocity.x;
        YVel = GetComponent<PlayerMovementController>().moveVelocity.y;
        ZVel = GetComponent<PlayerMovementController>().moveVelocity.z;


        Vector2 Velocitys = new Vector2(XVel, ZVel);
        //update XVel and ZVel to be relative to player rotation

        Vector2 rotatedVelocity = new Vector2(Velocitys.x * Mathf.Cos(-playerRotationOffset * Mathf.Deg2Rad) + Velocitys.y * Mathf.Sin(-playerRotationOffset * Mathf.Deg2Rad),
            Velocitys.y * Mathf.Cos(-playerRotationOffset * Mathf.Deg2Rad) - Velocitys.x * Mathf.Sin(-playerRotationOffset * Mathf.Deg2Rad));
        
        XVel = rotatedVelocity.x;
        ZVel = rotatedVelocity.y;       


        //update animator parameters XVel and YVel
        animator.SetFloat("VelX", XVel);
        animator.SetFloat("VelZ", ZVel);

        //update animator parameter isGrounded
        animator.SetBool("isGrounded", playerMovementController.isGrounded);


    }

    IEnumerator GetPlayerModel()
    {

        if (playerModel == null)
        {
            Debug.LogWarning("Player not found.");
            this.enabled = false;
            yield break;
        }

        if (playerModel.GetComponent<Animator>() == null)
        {
            Debug.LogWarning("Player does not have an Animator component.");
            this.enabled = false;
            yield break;
        }

        animator = playerModel.GetComponent<Animator>();
    }

    public void ReciveDash()
    {
        animator.SetTrigger("dash");
    }
}
