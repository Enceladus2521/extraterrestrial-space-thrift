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

    //set data
    public void SetExplosionData(float damage, float knockbackForce, float burnDps, float burnDuration, float explosionRadius, float explosionDamageFallOff)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.burnDps = burnDps;
        this.burnDuration = burnDuration;
        this.explosionRadius = explosionRadius;
        this.explosionDamageFallOff = explosionDamageFallOff;
    }

    
    private void Start()
    {
        //start coroutine
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        List<GameObject> rigidBodysHits = new List<GameObject>();
        List<GameObject> enemyHits = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                rigidBodysHits.Add(collider.gameObject);
            }
            if (collider.CompareTag("enemy"))
            {
                enemyHits.Add(collider.gameObject);
            }            
        }

        // Debug.Log("Explosion hit " + rigidBodysHits.Count + " rigidbodies and " + enemyHits.Count + " enemies");
        //Debug.Log("Explosion data is: damage: " + damage + " knockbackForce: " + knockbackForce + " burnDps: " + burnDps + " burnDuration: " + burnDuration + " explosionRadius: " + explosionRadius + " explosionDamageFallOff: " + explosionDamageFallOff);


        foreach (GameObject rigidBody in rigidBodysHits)
        {
            rigidBody.GetComponent<Rigidbody>().AddExplosionForce(knockbackForce, transform.position, explosionRadius);
        }

        foreach (GameObject enemy in enemyHits)
        {
                if (!enemy) continue;
                Vector3 enemyPosition = enemy.transform.position;
                float distance = Vector3.Distance(transform.position, enemyPosition);

                if (distance < explosionRadius)
                {
                    enemy.GetComponent<Rigidbody>().AddExplosionForce(knockbackForce, enemyPosition, explosionRadius);
                    float damageAmount = damage * (1 - (distance/explosionRadius) * explosionDamageFallOff);
                    enemy.GetComponent<EntityController>().TakeDamage(damageAmount);
                    yield return new WaitForSeconds(burnDuration);
                }
        }

        yield return null;
    }

}
