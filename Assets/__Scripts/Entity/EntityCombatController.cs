using UnityEngine;

public class EntityCombatController
{
    private EntityController entityController;
    private CombatStats stats;

    public EntityCombatController(EntityController entityController, CombatStats stats)
    {
        this.entityController = entityController;
        this.stats = stats;
    }

    public void Attack()
    {
        if (stats.canShoot)
        {
            Shoot();
        }
        else if (stats.canMelee)
        {
            MeleeAttack();
        }
    }

    private void Shoot()
    {
        if (stats.projectile != null)
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

    private void MeleeAttack()
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

    public void FixedUpdate()
    {
        // Implement any fixed update logic for combat, if needed
    }

    public void Update()
    {
        if (entityController.closestPlayer != null)
        {
            float distance = Vector3.Distance(entityController.transform.position, entityController.closestPlayer.transform.position);
            if (distance <= stats.attackRange)
            {
                Attack();
            }
        }
    }
}
