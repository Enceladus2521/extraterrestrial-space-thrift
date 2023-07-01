using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Interacter))]
[RequireComponent(typeof(Rigidbody))]

public class Weapon : MonoBehaviour
{
    public enum AmmoType
    {
        BulletA_pierce_ricochet_elec = 0,
        LaserA_burn_pierce_ricochet = 1,
        ProjA_burn_nkB_gravity_explo = 2,
        Sword_burn_nkB = 3,
        ElecA_burn_elec = 4
    };

    public bool dontDrop = false;

    private GameObject player;
    private bool isEquiped = false;
    [SerializeField] WeaponObj weaponObj;
    [SerializeField] Transform barrelTip;
    [SerializeField] float timeTillAutoReload = 1f;
    public IndividualWeaponData individualWeaponData;


    private bool lowAmmoIndicator = false;
    private GameObject lowAmmoIndicatorObj;
    public bool shooting = false;
    private bool readyToShoot = false;
    public bool reloading = false;
    public bool isPreloading = false;
    private bool onCooldown = false;
    private bool hasRelesedTrigger;
    public int ammoInClip;
    [Range(0, 5)]
    public float currentRecoil;
    private int weaponLevel; //Todo: load data

    GameObject meleeObj;


    [Header("Weapon Stats")]
    private GameObject ammoPrefab;
    private GameObject muzzleFlashPrefab;
    private GameObject hitEffectPrefab;


    [Header("Weapon Fire")]
    private bool FullAuto;
    private bool autoReload;
    private float weaponFireRate;
    private int projectilesPerShot;


    [Header("Weapon Reload")]
    private float weaponReloadTime;
    private int weaponClipSize;
    private int ammoCostPerShot;

    [Header("Weapon Recoil")]
    private float spreadIncreasePerShot;
    private float minSpread;
    private float maxSpread;
    private float recoilRecoveryRate;
    private float recoilRecoveryDelay;



    public void LoadWeaponData()
    {
        if (weaponObj != null)
        {
            shooting = false;
            readyToShoot = true;
            reloading = false;

            // load prefabs
            ammoPrefab = weaponObj.ammoPrefab;
            muzzleFlashPrefab = weaponObj.muzzleFlashPrefab;
            hitEffectPrefab = weaponObj.hitEffectPrefab;

            // load weapon stats
            FullAuto = weaponObj.FullAuto;
            autoReload = weaponObj.autoReload;
            weaponFireRate = weaponObj.weaponFireRate;
            projectilesPerShot = weaponObj.projectilesPerShot;

            // load weapon reload
            weaponReloadTime = weaponObj.weaponReloadTime;
            weaponClipSize = weaponObj.weaponClipSize;
            ammoCostPerShot = weaponObj.ammoCostPerShot;

            // load weapon recoil
            spreadIncreasePerShot = weaponObj.spreadIncreasePerShot;
            minSpread = weaponObj.minSpread;
            maxSpread = weaponObj.maxSpread;
            recoilRecoveryRate = weaponObj.recoilRecoveryRate;
            recoilRecoveryDelay = weaponObj.recoilRecoveryDelay;


            //TTODO: FIX: UiController.Instance.UpdateAmmo(player.GetComponent<PlayerInput>().playerIndex , player.GetComponent<PlayerStats>().GetAmmo(), ammoInClip, weaponObj.FullAuto); 
        }
    }

    
    private void Start()
    {
        if(weaponObj != null)
        {
            if ((int)weaponObj.weaponType == (int)WeaponObj.WeaponType.Sword)
            {                
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                SpawnSwordDamageDealer(meshFilter);
            }
            else if ((int)weaponObj.weaponType == (int)WeaponObj.WeaponType.GreatSword)
            {
                MeshFilter meshFilter = GetComponent<MeshFilter>();
                SpawnSwordDamageDealer(meshFilter);
            }            
        }

        


    }


    private void SpawnSwordDamageDealer(MeshFilter meshFilter)
    {
        GameObject newDamageDealer = new GameObject("DamageDealer");
        //set as child of weapon
        newDamageDealer.transform.SetParent(transform);
        //add mesh filter
        newDamageDealer.AddComponent<MeshFilter>().mesh = meshFilter.mesh;
        //add mesh collider
        newDamageDealer.AddComponent<MeshCollider>().sharedMesh = newDamageDealer.GetComponent<MeshFilter>().mesh;

        //dont do concave
        newDamageDealer.GetComponent<MeshCollider>().convex = true;       
        
        //set as trigger
        newDamageDealer.GetComponent<MeshCollider>().isTrigger = true;

        //add damage dealer script
        SwordDamageDealer damageDealer = newDamageDealer.AddComponent<SwordDamageDealer>();

        
        meleeObj = newDamageDealer;

        //deactivate damage dealer
        newDamageDealer.SetActive(false);
        

        //set position of new damage dealer to same as weapon position and rotation
        newDamageDealer.transform.position = transform.position;
        newDamageDealer.transform.rotation = transform.rotation;
        
    }


    private void Update()
    {
        if (player == null) return;
        if (!isEquiped) return;

        if ((int)weaponObj.weaponType == 5 || (int)weaponObj.weaponType == 6)
        {
            //-_- dont touch this it works            
        }
        else transform.LookAt(player.GetComponent<PlayerMovementController>().PlayerLookPosition());


        //check if not enough ammo to shoot
        if (ammoInClip < ammoCostPerShot)
        {
            if (!lowAmmoIndicator)
            {
                lowAmmoIndicatorObj = Instantiate(Resources.Load("autoLoad/PF_lowAmmo")) as GameObject;
                lowAmmoIndicatorObj.transform.SetParent(player.transform);
                lowAmmoIndicatorObj.transform.localPosition = new Vector3(0, 3, 0);
                lowAmmoIndicator = true;
            }
        }
        else
        {
            if (lowAmmoIndicator)
            {
                Destroy(lowAmmoIndicatorObj);
                lowAmmoIndicator = false;
            }
        }

        //if not shooting, set hasRelesedTrigger to true
        if (!shooting) hasRelesedTrigger = true;

        readyToShoot = CanShoot();

        //shoot if ready to shoot and not reloading and has more ammo than ammo cost per shot
        if (!FullAuto && readyToShoot && hasRelesedTrigger && shooting || FullAuto && readyToShoot && shooting)
        {
            StopCoroutine(Reload());
            Shoot();
            StartCoroutine(CoolDown());
            ammoInClip -= ammoCostPerShot;
            UiController.Instance.UpdateAmmo(player.GetComponent<PlayerInput>().playerIndex , player.GetComponent<PlayerStats>().GetAmmo(), ammoInClip, weaponObj.FullAuto); 
            //Debug.Log("Ammo in clip: " + ammoInClip);
        }
        else if (!shooting && ammoInClip < weaponClipSize && !reloading && autoReload && !isPreloading)
        {            
            StartCoroutine(Reload(timeTillAutoReload));
        }

        //recoil recovery
        if (currentRecoil > 0 && !shooting)
        {
            currentRecoil -= recoilRecoveryRate * Time.deltaTime;
        }
        else if (currentRecoil < 0)
        {
            currentRecoil = 0;
        }
    }

    private void Shoot()
    {
        CoolDown(); //start cooldown
        hasRelesedTrigger = false; //set hasRelesedTrigger to false
        
        if ((int)weaponObj.weaponType == 5 || (int)weaponObj.weaponType == 6)
        {
            //-_- dont touch this it works
            StartCoroutine(EnableSwordTrigger(weaponFireRate));
        }
        else if (weaponObj.ammoPrefab != null) ShootProjectile();
        else ShootRaycast();

        //play animation
        player.GetComponent<PlayerAnimationController>().ReciveShoot();

        //to do: add recoil
        currentRecoil += spreadIncreasePerShot; //increase recoil
    }

    #region Raycast
    private void ShootRaycast()
    {        
        //if muzzle flash prefab is not null, instantiate muzzle flash prefab at barrel tip position and rotation and set scale to weapon muzzle flash scale
        if (muzzleFlashPrefab != null) Instantiate(muzzleFlashPrefab, barrelTip.position, barrelTip.rotation).gameObject.transform.localScale = weaponObj.muzzleFlashScale;

        for (int i = 0; i < projectilesPerShot; i++)
        {
            //calculate spread
            Vector3 spreadVector = CalculateSpread();

            //calculate direction
            Vector3 direction = barrelTip.forward + spreadVector;

            //raycast---------------------------------------------------------------------------------------            
            List<RaycastHit> hits = new List<RaycastHit>();
            //raycastall to get all hits
            hits.AddRange(Physics.RaycastAll(barrelTip.position, direction, weaponObj.weaponRange));

            if (hits.Count > 0)
            {
                //sort hits by distance
                hits.Sort((x, y) => x.distance.CompareTo(y.distance));

                //get first hit
                RaycastHit hit = hits[0];

                //hit
                OnHit(hit, direction, weaponObj.weaponDamage);

                //do tracer
                SpawnTracer(barrelTip.position, hit.point);

                //do piercing
                if (weaponObj.piercingAmount > 0) PierceEnemy(hit, direction, hits);
            }
            else
            {
                SpawnTracer(barrelTip.position, barrelTip.position + direction * weaponObj.weaponRange);
            }
        }
    }

    private void PierceEnemy(RaycastHit hit, Vector3 direction, List<RaycastHit> hits)
    {
        //do piercing                  
        if (weaponObj.piercingAmount > 0 && hits.Count > 1)
        {
            for (int j = 1; j < weaponObj.piercingAmount; j++)
            {
                if (hits[j].collider.gameObject == null) return;

                Transform lastHit = hits[j - 1].collider.gameObject.transform;
                RaycastHit currentHit = hits[j];

                //do tracer 
                SpawnTracer(lastHit.position, currentHit.point);

                //do damage
                float damage = weaponObj.weaponDamage;
                damage = damage * (weaponObj.piercingDamageFallOff * j);

                //do hit
                OnHit(currentHit, direction, damage);

                //Todo: add burn to entity spawn burn effect Prefab and child it to entity set data for burn effect                   
            }
        }
    }

    private void SpawnTracer(Vector3 tracerStartPoint, Vector3 hitObject)
    {
        if (weaponObj.tracerPrefab != null)
        {
            GameObject tracer = Instantiate(weaponObj.tracerPrefab, tracerStartPoint, Quaternion.identity);
            // Debug.Log("TracerData: StartPos: " + tracerStartPoint + " EndPos: " + hitObject);
            tracer.GetComponent<BulletTracer>().SetStartAndEndPos(tracerStartPoint, hitObject, weaponObj.tracerColor);
        }
    }

    private void OnHit(RaycastHit hit, Vector3 direction, float damage)
    {
        if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

        if (weaponObj.explosionPrefab == null) AddKnockback(hit, direction); //do knockback if no explosion

        Explode(hit, damage);
        //on entity hit
        if (!hit.collider.gameObject.CompareTag("entity")) return;


        // try to get EntityController
        EntityController entity = hit.collider.gameObject.GetComponent<EntityController>();
        if (entity != null) entity.TakeDamage(damage);

        //Todo: add burn to entity spawn burn effect Prefab and child it to entity set data for burn effect


    }


    private void AddKnockback(RaycastHit hit, Vector3 direction)
    {
        //do knockback
        if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
        {

            hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(direction * weaponObj.knockbackForce, ForceMode.Impulse);
        }
    }

    private void Explode(RaycastHit hit, float damage)
    {
        if (weaponObj.explosionPrefab == null) return;
        GameObject explosion = Instantiate(weaponObj.explosionPrefab, hit.point, Quaternion.identity);

        explosion.GetComponent<Explosion>().SetExplosionData(damage, weaponObj.knockbackForce, weaponObj.burnDps, weaponObj.burnDuration, weaponObj.explosionRadius, weaponObj.explosionDamageFallOff);
    }
    #endregion

    #region Projectile
    private void ShootProjectile()
    {
        //if muzzle flash prefab is not null, instantiate muzzle flash prefab at barrel tip position and rotation and set scale to weapon muzzle flash scale
        if (muzzleFlashPrefab != null) Instantiate(muzzleFlashPrefab, barrelTip.position, barrelTip.rotation).gameObject.transform.localScale = weaponObj.muzzleFlashScale;
        for (int i = 0; i < projectilesPerShot; i++)
        {

            //calculate spread
            Vector3 spreadVector = CalculateSpread();

            //calculate direction
            Vector3 direction = barrelTip.forward + spreadVector;



            //instantiate projectile from barrel tip
            GameObject projectile = Instantiate(weaponObj.ammoPrefab, barrelTip.position, barrelTip.rotation);

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            float force;
            float gravityMultiplier;
            if ((int)weaponObj.projectileMovementType == 0)
            {
                force = weaponObj.constantForce;
                gravityMultiplier = 0;
            }
            else
            {
                force = weaponObj.impulseForce;
                gravityMultiplier = weaponObj.gravityMultiplier;
            }


            projectileScript.SetProjectileData(weaponObj.weaponDamage, direction, //set damage and direction
            weaponObj.lifeTime, weaponObj.sticky, //set life time and sticky
            weaponObj.explodeOnImpact, weaponObj.bounceAmount, (int)weaponObj.projectileMovementType, //set explode on impact and bounce amount
            force, gravityMultiplier); //set force and gravity multiplier

            projectileScript.SetEffectData(weaponObj.knockbackForce, weaponObj.burnDps, //set knockback force and burn dps
            weaponObj.burnDuration, weaponObj.explosionPrefab, //set burn duration and explosion prefab
            weaponObj.explosionRadius, weaponObj.explosionDamageFallOff); //set explosion radius and damage falloff

        }
    }
    #endregion


    IEnumerator EnableSwordTrigger(float SliceTime)
    {
        meleeObj.SetActive(true);        
        yield return new WaitForSeconds(SliceTime); 
        meleeObj.SetActive(false);       
    }
    

    public void OnSliceHit(GameObject hitObject)
    {
        //if hit object is player
        if (hitObject.CompareTag("Player")) return;

        //if hit object is entity
        if (hitObject.CompareTag("entity"))
        {
            EntityController entity = hitObject.GetComponent<EntityController>();
            entity.TakeDamage(weaponObj.weaponDamage);

            //Todo: add burn to entity spawn burn effect Prefab and child it to entity set data for burn effect
        }

        if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, hitObject.transform.position, Quaternion.LookRotation(hitObject.transform.forward));

        //if explosion prefab is not null, instantiate explosion prefab at hit object position
        if (weaponObj.explosionPrefab != null) Instantiate(weaponObj.explosionPrefab, hitObject.transform.position, Quaternion.identity);
        else if (hitObject.GetComponent<Rigidbody>() != null)
        {
            if (weaponObj.knockbackForce > 0) hitObject.GetComponent<Rigidbody>().AddForce(transform.forward * weaponObj.knockbackForce, ForceMode.Impulse);
        }
    }

    private Vector3 CalculateSpread()
    {
        Vector3 spreadVector;

        //Spread
        float x = Random.Range(-minSpread - currentRecoil, maxSpread + currentRecoil);

        //check if spread is within min and max spread
        if (x > maxSpread) x = maxSpread;
        else if (x < -minSpread) x = -minSpread;

        spreadVector = new Vector3(x, 0, 0);

        return spreadVector;
    }

    private IEnumerator CoolDown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(weaponFireRate);
        onCooldown = false;
    }

    private bool CanShoot()
    {
        return !onCooldown && ammoInClip >= ammoCostPerShot;
    }

    public bool CanReload()
    {
        return !reloading && ammoInClip < weaponClipSize;
    }

    public IEnumerator Reload(float timeTillAutoReload = 0)
    {
        if (player.GetComponent<PlayerStats>().GetAmmo() < weaponClipSize - ammoInClip)
        {
            Debug.Log("Not enough ammo to reload");
            //Todo: no more ammo on player
            yield break;
        }
        isPreloading = true;
        yield return new WaitForSeconds(timeTillAutoReload);

        reloading = true;
        yield return new WaitForSeconds(weaponReloadTime);


        
        reloading = false;
        isPreloading = false;
        ammoInClip = weaponClipSize;
        if ((int)weaponObj.weaponType == 5 || (int)weaponObj.weaponType == 6)
        {
            //-_- dont touch this it works   
            yield break;       
        }
        player.GetComponent<PlayerStats>().TakeAmmo(weaponClipSize - ammoInClip);
        
       

        
        UiController.Instance.UpdateAmmo(player.GetComponent<PlayerInput>().playerIndex, player.GetComponent<PlayerStats>().GetAmmo(), ammoInClip, weaponObj.FullAuto);       
    }


    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void SetAmmoInClip(int ammoInClip, int level)
    {
        this.ammoInClip = ammoInClip;
        this.weaponLevel = level;
        
       
    }

    public int GetAmmoInClip()
    {
        return ammoInClip;
    }



    public void SetShooting(bool shooting)
    {
        this.shooting = shooting;
    }

    private void OnValidate()
    {
        if (weaponObj != null)
        {
            Interacter interacter = GetComponent<Interacter>();
            interacter.AutoTrigger = false;
            interacter.InteractOnce = true;
            interacter.events.AddListener(Pickup);

            interacter.SetInteractText("Press F or X to pickup " + weaponObj.name);
        }
    }
    private void OnEnable()
    {
        if (weaponObj != null)
        {
            Interacter interacter = GetComponent<Interacter>();
            interacter.events.AddListener(Pickup);

            interacter.SetInteractText("Press F or X to pickup " + weaponObj.name);

            individualWeaponData = new IndividualWeaponData(0, 0, 0);
        }
    }
    public void Pickup()
    {
        //get player
        GameObject player = GetComponent<Interacter>().Player;


        //add to player
        if (player.GetComponent<WeaponController>().PickUpWeapon(weaponObj, individualWeaponData, dontDrop))
        {
            //remove from world
            Destroy(gameObject);
        }
        UiController.Instance.UpdateAmmo(player.GetComponent<PlayerInput>().playerIndex, player.GetComponent<PlayerStats>().GetAmmo(), ammoInClip, weaponObj.FullAuto); 
    }
    public void SetIndividualWeaponData(IndividualWeaponData data)
    {
        individualWeaponData = data;

    }

    public IndividualWeaponData GetIndividualWeaponData()
    {
        return individualWeaponData;

    }

    public void SetEquipped(bool equipped)
    {
        this.isEquiped = equipped;

        if (equipped)
        {
            //remove pickup Text
            GetComponent<Interacter>().SetInteractText("");
            

        }
        else
        {
            //add pickup Text
            GetComponent<Interacter>().SetInteractText("Press F or X to pickup " + weaponObj.name);

        }

        if (!equipped && lowAmmoIndicator)
        {
            Destroy(lowAmmoIndicatorObj);
            lowAmmoIndicator = false;
        }

    }



}
