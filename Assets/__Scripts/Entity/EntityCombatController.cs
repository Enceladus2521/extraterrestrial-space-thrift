using UnityEngine;

public class EntityCombatController : MonoBehaviour
{
    public EntityController entityController;
    public CombatStats stats;
    public GameObject target;

    private void Start()
    {
        if (entityController == null)
        {
            entityController = GetComponent<EntityController>();
        }
    }

    public void Attack(GameObject target)
    {
        if (stats.canShoot)
        {
            Shoot(target);
        }
        else if (stats.canMelee)
        {
            MeleeAttack(target);
        }
    }

    private void Shoot(GameObject target)
    {
        if (stats.projectile != null && target != null)
        {
            // Instantiate the bullet prefab
            GameObject bullet = GameObject.Instantiate(stats.projectile, entityController.transform.position, entityController.transform.rotation);

            // Add BulletController component to the bullet object if not present
            if (bullet.GetComponent<BulletController>() == null)
            {
                bullet.AddComponent<BulletController>();
            }

            // Perform shooting logic (e.g., apply force to the bullet, set damage, etc.)
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.SetDamage(stats.shootingDamage);
            bulletController.Fire(entityController.transform.forward);
        }
    }

    private void MeleeAttack(GameObject target)
    {
        Collider[] colliders = Physics.OverlapSphere(entityController.transform.position, stats.attackRange);

        foreach (Collider collider in colliders)
        {
            // Check if the collider belongs to an enemy entity
            EntityController enemy = collider.GetComponent<EntityController>();
            if (enemy != null && enemy != entityController)
            {
                // Apply damage to the enemy entity
                enemy.TakeDamage(stats.meleeDamage);
            }
        }
    }

    public void Update()
    {
        if (entityController.GetClosestPlayer() != null)
        {
            float distance = Vector3.Distance(entityController.transform.position, entityController.GetClosestPlayer().transform.position);
            if (distance <= stats.attackRange)
            {
                Attack(target);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (entityController == null || stats == null)
            return;

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(entityController.transform.position, stats.attackRange);

        // Draw additional gizmos here if needed
    }

    private void OnDrawGizmosSelected()
    {
        // Draw selected gizmos here if needed
    }
}
