using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponObj : ScriptableObject
{

    [Header("Weapon Type")]
    public WeaponType weaponType;

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

    public WeaponOffset weaponOffset;
    public enum WeaponOffset
    {
        defaultWeaponOffset = 0,
        customWeaponOffset = 1        
    };

    [Header("Weapon Info")]
    public string weaponName;
    public GameObject weaponPrefab;



    [Header("Weapon Offset")]

    [HideInInspector] public Vector3 defaultPositionOffset = new Vector3(0, 0, 0);
    [HideInInspector] public Vector3 defaultRotationOffset = new Vector3(0, 0, 0);
    [HideInInspector] public Vector3 defaultScaleOffset = new Vector3(1, 1, 1);   


    public Vector3 customPositionOffset;
    public Vector3 customRotationOffset;
    public Vector3 customScaleOffset = Vector3.one;





    [Header("Weapon Stats")]
    public int weaponDamage;    
    public float weaponRange;
    public float weaponFireRate;
    public float weaponReloadTime;
    public int weaponMagSize;
    public int ammoCostPerShot;

    
    
    [Header("Gun Ammo")]
    public GameObject bulletPrefab;
    public bool canRicochet;
    public int ricochetAmount;    

    [Header("Laser Ammo")]
    public GameObject laserPrefab; 
    public bool canBurn;
    public float burnDps;   

    [Header("Shotgun Ammo")]
    public GameObject shotgunPrefab;
    public int shotgunPellets;
    public float shotgunSpreadAngle;
    public bool canNockBack;
    public float knockBackForce;

    [Header("Sniper Ammo")]
    public GameObject sniperPrefab; 
    public bool canPierce;
    public int pierceAmount;   


    
    [Header("Projectile Ammo")]    
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileLifeTime; 
    public int explosionRadius;


    [Header("Electric Ammo")]    
    public GameObject electricPrefab;
    public int electricChainAmount;
    public float electricChainRange;
    public float electricChainDamageFallOff = 0.7f;













}
