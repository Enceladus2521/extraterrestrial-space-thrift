using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;

    [Header("Player Model with Animator")]
    [SerializeField] GameObject playerModel;

    public float XVel;
    public float YVel;
    
    private Rigidbody rb;
    void Start()
    {       
        rb = GetComponent<Rigidbody>();
        StartCoroutine(GetPlayerModel());
    }

    // Update is called once per frame
    void Update()
    {
        //update animator parameters XVel and YVel
        animator.SetFloat("XVel", rb.velocity.x);
        animator.SetFloat("YVel", rb.velocity.z);

        XVel = rb.velocity.x;
        YVel = rb.velocity.z;


    }

    IEnumerator GetPlayerModel()
    {
        
        if(playerModel == null)
        {
            Debug.LogWarning("Player not found.");
            this.enabled = false;
            yield break;
        }

        if(playerModel.GetComponent<Animator>() == null)
        {
            Debug.LogWarning("Player does not have an Animator component.");
            this.enabled = false;
            yield break;
        }

        animator = playerModel.GetComponent<Animator>();
    }
}
