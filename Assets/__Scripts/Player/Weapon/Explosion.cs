using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    
    private float damage = 0;
    private float knockbackForce = 0f;
    private float burnDps = 0f;
    private float burnDuration = 0f;

    private float explosionRadius = 0f;

    private float explosionDamageFallOff = 0.7f;

    
    public bool damagePlayer;

    //set data
    public void SetExplosionData(float damage, float knockbackForce, float burnDps, float burnDuration, float explosionRadius, float explosionDamageFallOff, bool damagePlayer)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.burnDps = burnDps;
        this.burnDuration = burnDuration;
        this.explosionRadius = explosionRadius;
        this.explosionDamageFallOff = explosionDamageFallOff;
        this.damagePlayer = damagePlayer;
        StartCoroutine(Explode());
    }

    
    

    IEnumerator Explode()
    {
        List<GameObject> rigidBodysHits = new List<GameObject>();
        List<GameObject> entityHits = new List<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        
        foreach (Collider collider in colliders)
        {
            
            if (collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                rigidBodysHits.Add(collider.gameObject);
            }
            if (collider.CompareTag("entity"))
            {
                entityHits.Add(collider.gameObject);
            }    
            if (collider.CompareTag("Player"))
            {       
                if(!damagePlayer) continue;
                float damageAmount = damage * (1 - (Vector3.Distance(transform.position, collider.transform.position)/explosionRadius) * explosionDamageFallOff);
                collider.gameObject.GetComponent<PlayerStats>().TakeDamage(damageAmount);
            }    
            if(collider.CompareTag("Enemy"))
            {
                float damageAmount = damage * (1 - (Vector3.Distance(transform.position, collider.transform.position)/explosionRadius) * explosionDamageFallOff);
                collider.gameObject.GetComponent<EnemyController>().TakeDamage(damageAmount);
            }

        }

        

        // Debug.Log("Explosion hit " + rigidBodysHits.Count + " rigidbodies and " + entityHits.Count + " enemies");
        //Debug.Log("Explosion data is: damage: " + damage + " knockbackForce: " + knockbackForce + " burnDps: " + burnDps + " burnDuration: " + burnDuration + " explosionRadius: " + explosionRadius + " explosionDamageFallOff: " + explosionDamageFallOff);


        foreach (GameObject rigidBody in rigidBodysHits)
        {
            rigidBody.GetComponent<Rigidbody>().AddExplosionForce(knockbackForce, transform.position, explosionRadius);
        }

        foreach (GameObject entity in entityHits)
        {
                if (!entity) continue;
                Vector3 entityPosition = entity.transform.position;
                float distance = Vector3.Distance(transform.position, entityPosition);

                if (distance < explosionRadius)
                {
                    entity.GetComponent<Rigidbody>().AddExplosionForce(knockbackForce, entityPosition, explosionRadius);
                    float damageAmount = damage * (1 - (distance/explosionRadius) * explosionDamageFallOff);
                    Debug.Log("hit with damage: " + damage);
                    EntityController entityController = entity.GetComponent<EntityController>();
                    entityController.TakeDamage(damageAmount);
                    yield return new WaitForSeconds(burnDuration);
                }
        }



        yield return null;
    }

}
