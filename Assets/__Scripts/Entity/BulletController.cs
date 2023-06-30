using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float damage;
    private Rigidbody rb;

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void Fire(Vector3 direction)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        // Apply force to the bullet in the specified direction
        rb.AddForce(direction * 10f, ForceMode.Impulse);

        // Destroy the bullet after a certain time (e.g., 5 seconds)
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet collided with an entity
        EntityController entity = collision.gameObject.GetComponent<EntityController>();
        if (entity != null)
        {
            // Apply damage to the entity
            entity.TakeDamage(damage);
        }else {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null)
                player.TakeDamage(damage);
        }

        // Destroy the bullet upon collision
        Destroy(gameObject);
    }
}
