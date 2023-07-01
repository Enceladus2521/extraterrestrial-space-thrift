using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
     private float damage = 1f;  
     private bool explodeOnImpact = false;  
       
    
    float knockbackForce = 0f;

    
    public void SetProjectileData(float damage, float lifeTime, bool explodeOnImpact, float force, float knockbackForce)
    {
        this.damage = damage;     
        this.explodeOnImpact = explodeOnImpact;
        
        this.knockbackForce = knockbackForce;

        StartCoroutine(DestroyAfterTime(lifeTime));

        GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
    }

    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStats>().TakeDamage(damage);
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * knockbackForce, ForceMode.Impulse);
            
            
            
            Destroy(gameObject);
        }
        else if (explodeOnImpact)
        {
            
            
            Destroy(gameObject);
        }
       
    }

    IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        
        Destroy(gameObject);
    }


}
