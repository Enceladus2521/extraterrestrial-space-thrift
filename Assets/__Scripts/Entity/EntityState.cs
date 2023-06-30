using UnityEngine;
using System.Collections.Generic;

public enum MaterialType
{
    Metal,
    Meat
}

public class EntityState : MonoBehaviour
{

    [SerializeField]
    public MovementStats movementStats;

    [SerializeField]
    public CombatStats combatStats;

    [SerializeField]
    public HealthStats healthStats;

    [SerializeField]
    public InventoryStats inventoryStats;



    [SerializeField]
    private MaterialType materialType;

    public void LoadPrefab(EntityPrefab prefab)
    {
        movementStats = prefab.movementStats;
        combatStats = prefab.combatStats;
        healthStats = prefab.healthStats;
        inventoryStats = prefab.inventoryStats;
        materialType = prefab.materialType;
    }

    void OnDrawGizmos()
    {
        // render attack range as circle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, movementStats.maintainDistance);
        
        // render view range as circle
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, movementStats.viewRange);

        // render view angle as two lines
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * movementStats.viewRange);
        // view angle
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * movementStats.viewRange);

        
    }

}


[System.Serializable]
public class InventoryStats
{
    public int maxCapacity;
    public int currentCapacity;
}

[System.Serializable]
public class MovementStats
{
    public float maxSpeed;

    [Range(0, 10)]
    public float speed;

    [Range(0, 10)]
    public float flyingAltitude;

    [Range(0, 10)]
    public float maintainDistance;

    [Range(0, 360)]
    public float viewAngle;

    [Range(0, 10)]
    public float turnSpeed;

    [Range(0, 10)]
    public float viewRange;

    public bool isStunned;

}

[System.Serializable]
public class CombatStats
{
    public bool canShoot;
    public float shootingDamage;
    public bool canMelee;
    public float meleeDamage;
    public float attackRange;
    public GameObject projectile;
}

[System.Serializable]
public class HealthStats

{   
    [SerializeField]
    private float maxArmor;
    [SerializeField]
    private float maxHealth;
    
    [SerializeField]
    bool isAlive;
    
    
    [SerializeField]
    float health; 
    [SerializeField]
    float armor;
    [SerializeField]
    float healthRegenerationRate;
    



    public void ResetEntity()
    {
        health = maxHealth;
        armor = maxArmor;
    }

    public void TakeDamage(float damage)
    {
        // the more armor the more damgae reduced
        health -= damage;
        if (health <= 0f)
        {
            isAlive = false;
            health = 0f;
        }else {
            isAlive = true;
        }
    }
    // getter for health
    public float Health { get { return health; } }
    public bool IsAlive { get { return isAlive; } }
}