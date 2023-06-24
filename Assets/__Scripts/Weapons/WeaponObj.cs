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

    [Header("Weapon Info")]
    public string weaponName;
    public GameObject weaponPrefab;
    public Vector3 weaponPositionOffset;
    public Vector3 weaponRotationOffset;
    public Vector3 weaponScaleOffset = Vector3.one;





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
    public bool ProjetileWeapon;
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
