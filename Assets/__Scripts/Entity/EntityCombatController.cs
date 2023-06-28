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
        else if (stats.hasMelee)
        {
            MeleeAttack();
        }
    }

    private void Shoot()
    {
        // Instantiate the bullet prefab and perform shooting logic
        // You can define your own implementation here based on the requirements
    }

    private void MeleeAttack()
    {
        // Perform melee attack logic
        // You can define your own implementation here based on the requirements
    }

    public void FixedUpdate()
    {
        
    }

    public void Update()
    {
        
    }
}
