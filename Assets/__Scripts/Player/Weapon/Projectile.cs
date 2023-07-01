using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private float damage = 1f;
    private float lifeTime = 5f;
    private bool isSticky = false;
    private bool explodeOnImpact = false;
    private int bounceAmount = 0;
    private ProjectileMovementType projectileMovementType;
    private float force;
    private float gravityMultiplier = 0f;
   

    float knockbackForce = 0f;
    float burnDps = 0f;
    float burnDuration = 0f;

    GameObject explosionPrefab;
    float explosionRadius = 0f;
    float explosionDamageFallOff = 0.7f;



    private bool OnBounceCooldown = false;
    private int currentBounceAmount = 0;

    private Vector3 direction;

    public void SetProjectileData(float damage, Vector3 direction, float lifeTime, bool isSticky,bool explodeOnImpact, int bounceAmount, int projectileMovementType, float force, float gravityMultiplier)
    {
        this.damage = damage;
        this.direction = direction;
        this.lifeTime = lifeTime; //done
        this.isSticky = isSticky;
        this.explodeOnImpact = explodeOnImpact;
        this.bounceAmount = bounceAmount;
        this.projectileMovementType = (ProjectileMovementType)projectileMovementType;
        this.force = force;
        this.gravityMultiplier = gravityMultiplier;
    }

    public void SetEffectData(float knockbackForce, float burnDps, float burnDuration, GameObject explosionPrefab, float explosionRadius, float explosionDamageFallOff)
    {
        this.knockbackForce = knockbackForce;
        this.burnDps = burnDps;
        this.burnDuration = burnDuration;
        this.explosionPrefab = explosionPrefab;
        this.explosionRadius = explosionRadius;
        this.explosionDamageFallOff = explosionDamageFallOff;
    }


    
    
    private void Start()
    {
        StartCoroutine(DestroyAfterTime(lifeTime));

        if(projectileMovementType == ProjectileMovementType.ConstantForce)
        {
            StartCoroutine(ProjectileMover());
        }
        else if(projectileMovementType == ProjectileMovementType.ImpulseForce)
        {
            GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
        }

    }
    
    

    IEnumerator ProjectileMover()
    {
        //freze the rotation of the projectile
        GetComponent<Rigidbody>().freezeRotation = true;
        //disable gravity
        GetComponent<Rigidbody>().useGravity = false;
        while (true)
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Force);
            //cap the velocity of the projectile
            GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, force);
            
            yield return null;
        }
    }

    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("entity") && explosionPrefab == null)
        {
            Debug.Log("hit with damage: " + damage);
            EntityController entityController = other.gameObject.GetComponent<EntityController>();
            entityController.TakeDamage(damage);
            DestroySelf();
        }
        else if(other.gameObject.CompareTag("Enemy") && explosionPrefab == null)
        {
            Debug.Log("hit with damage: " + damage);
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            enemyController.TakeDamage(damage);
            DestroySelf();
        }
        else if(bounceAmount > 0)
        {
            if (OnBounceCooldown)
            {
                return;
            }
            
            StartCoroutine(BounceCooldown(0.1f));
            //get the normal of the collision and calculate the new direction
            Vector3 newDirection = Vector3.Reflect(transform.forward, other.contacts[0].normal);
            //rotate the object to face the new direction
            direction = newDirection;
            transform.rotation = Quaternion.LookRotation(direction);
            bounceAmount--; 
            
            //calculate the bounce force based on the current force and current bounce amount (the more bounces the less force)
            float bounceForce = force * (1 - (currentBounceAmount / bounceAmount));
            //add the force to the rigidbody
            GetComponent<Rigidbody>().AddForce(transform.forward * bounceForce, ForceMode.Impulse);
            //increase the current bounce amount 
                      
            currentBounceAmount++;
        }
        else if (isSticky)
        {
            //set the parent of the projectile to the object it collided with
            transform.SetParent(other.transform);
            //set the rigidbody to kinematic so it doesn't move            
            GetComponent<Rigidbody>().isKinematic = true;
        }

        if (explodeOnImpact)
        {
            DestroySelf();
        }      
        
    }

    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        DestroySelf();
    }

    IEnumerator BounceCooldown(float time)
    {
        OnBounceCooldown = true;
        yield return new WaitForSeconds(time);
        OnBounceCooldown = false;
    }

    private void DestroySelf()
    {
        StopAllCoroutines();
        if (explosionPrefab != null)
        {
            GameObject explo = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explo.GetComponent<Explosion>().SetExplosionData(damage,knockbackForce,burnDps,burnDuration,explosionRadius,explosionDamageFallOff);
        }
        

        Destroy(gameObject);
    }
   

}

public enum ProjectileMovementType
{
    ConstantForce = 0,
    ImpulseForce = 1,
};
