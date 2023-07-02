using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponObj : ScriptableObject
{

    [Header("Weapon Type")]
    public WeaponType weaponType;
    

    [Header("Weapon Info")]
    public string weaponName;
    public GameObject weaponPrefab;



    [Header("Weapon Offset")]
    public WeaponOffset weaponOffset;
    public enum WeaponOffset
    {
        defaultWeaponOffset = 0,
        customWeaponOffset = 1
    };

    [HideInInspector] public Vector3 defaultPositionOffset = new Vector3(0.149f,0.035f, -0.061f);
    [HideInInspector] public Vector3 defaultRotationOffset = new Vector3(32.843f, 107.173f, -77.405f);
    [HideInInspector] public Vector3 defaultScaleOffset = new Vector3(1, 1, 1);


    public Vector3 customPositionOffset;
    public Vector3 customRotationOffset;
    public Vector3 customScaleOffset = Vector3.one;



    [Header("-----------------------Weapon Stats-----------------------")]
    [Header("Weapon Behaviour")]
    public bool FullAuto = false;
    public bool autoReload = false;
    public float weaponFireRate = 0.1f;
    public int projectilesPerShot = 1;


    public int weaponDamage = 5;
    public float weaponRange = 100f;



    [Header("-----------------------Weapon Reload-----------------------")]

    [Range(0, 5)]
    public float weaponReloadTime = 0.5f;
    public int weaponClipSize = 1;
    public int ammoCostPerShot = 1;


    [Header("Weapon Recoil")]
    public float spreadIncreasePerShot = 0.1f;
    public float minSpread = 0;
    public float maxSpread = 0;    
    public float recoilRecoveryRate = 0.1f;
    public float recoilRecoveryDelay = 0.3f;

    [Header("-----------------------Ammo-----------------------")]
    [Header("Ammo Required when ammoType is Projectile")]
    public GameObject ammoPrefab;
      


    public GameObject muzzleFlashPrefab;
    public Vector3 muzzleFlashScale = new Vector3(1, 1, 1); 
    public GameObject hitEffectPrefab;       
    public Vector3 hitEffectScale = new Vector3(1, 1, 1);   

    
    [Header("Weapon Sound")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    



    [Header("Ammo Behaviour")]
    [Header("NockBack for Raycast and Projectile")]    
    public float knockbackForce = 0f;

    [Header("Burn Damage for Raycast and Projectile")]    
    public float burnDps = 0f;
    public float burnDuration = 0f;

    [Header("Explosive Explosion for Raycast and Projectile")]
    public GameObject explosionPrefab;
    public float explosionRadius = 0f;    
    public float explosionDamageFallOff = 0.7f;

    [Header("Raycast Specific------------------------")]  

    public GameObject tracerPrefab;   
    public Material tracerColor;

    [Header("Piercing")]
    public int piercingAmount = 0;    
    public float piercingDamageFallOff = 0.7f;   


    [Header("Projectile Specific------------------------")]

    public float lifeTime = 5f;
    public bool sticky = false;
    public bool explodeOnImpact = false;
    public int bounceAmount = 0;
    public GameObject trailPrefab;
    
    
    public ProjectileMovementType projectileMovementType;
    //if projectileMovementType is ConstantForce       
    public float constantForce = 5f;
    
    //if projectileMovementType is ImpulseForce
    public float impulseForce = 5f;         
    public float gravityMultiplier = 1f;  
    

    

    public enum ProjectileMovementType
    {
        ConstantForce = 0,
        ImpulseForce = 1,
    };




    public enum WeaponType
    {
        Gun = 0,
        Laser = 1,
        Shotgun = 2,
        Sniper = 3,
        Projectile = 4,
        Sword = 5,
        GreatSword = 6,
        Electric = 7
    };
}


