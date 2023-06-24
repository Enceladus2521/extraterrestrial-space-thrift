using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(WeaponController))]

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private WeaponType currentWeaponType;
    
    public enum WeaponType
    {
        Gun = 0,
        Laser = 1,
        Shotgun = 2,
        Sniper = 3,
        Projectile = 4,
        Sword = 5,
        GreatSword = 6,
        Electric = 7,
        nothing = 8      
    };
   
    Animator animator;

    PlayerMovementController playerMovementController;
    WeaponController weaponController;

    [Header("Player Model with Animator")]
    [SerializeField] GameObject playerModel;

    public float XVel;

    public float YVel;
    public float ZVel;

    public float playerRotationOffset = 0f;


    void Start()
    {
        weaponController = GetComponent<WeaponController>();

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



        //update layer weight based on weapon type 
        // Base Layer = 0, rifle = 1,  sword = 2, greatWeapon = 3, rocketThrower = 4

        //get current weapon type if null reset layer weights
        if (weaponController.GetCurrentWeapon() == null)
        {
            ResetLayerWaits();
            currentWeaponType = WeaponType.nothing;
            return;
        }
        WeaponObj currentWeapon = weaponController.GetCurrentWeapon();
        switch((int)currentWeapon.weaponType) 
        {
            case 0: //gun
                ActivateLayer(1);
                currentWeaponType = WeaponType.Gun;
                break;
            case 1: //laser
                ActivateLayer(1);
                currentWeaponType = WeaponType.Laser;
                break;
            case 2: //shotgun
                ActivateLayer(1);
                currentWeaponType = WeaponType.Shotgun;
                break;
            case 3: //sniper
                ActivateLayer(1);
                currentWeaponType = WeaponType.Sniper;
                break;
            case 4: //projectile
                ActivateLayer(4);
                currentWeaponType = WeaponType.Projectile;
                break;
            case 5: //sword
                ActivateLayer(2);
                currentWeaponType = WeaponType.Sword;
                break;
            case 6: //greatsword
                ActivateLayer(3);
                currentWeaponType = WeaponType.GreatSword;
                break;
            case 7: //electric
                ActivateLayer(1);
                currentWeaponType = WeaponType.Electric;
                break;
            default:
                ActivateLayer(0);  
                currentWeaponType = WeaponType.nothing;         
                break;

            
        }

        


    }

    private void ResetLayerWaits()
    {
        animator.SetLayerWeight(0, 1);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);
        animator.SetLayerWeight(3, 0);
        animator.SetLayerWeight(4, 0);        
    }

    private void ActivateLayer(int layerNumber)
    {
        ResetLayerWaits();
        animator.SetLayerWeight(layerNumber, 1);
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

    public void ReciveShoot()
    {
        animator.SetTrigger("singleFire");
    }

    public void ReciveReload()
    {
        animator.SetTrigger("reload");
    }

    public void ReciveDash()
    {
        animator.SetTrigger("dash");
    }
}
