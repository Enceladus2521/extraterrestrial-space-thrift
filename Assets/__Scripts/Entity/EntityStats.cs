using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public enum MaterialType
{
    Metal,
    Meat
}

[CreateAssetMenu(menuName = "Enemy Stats")]
public class EntityStats : ScriptableObject
{
    public MovementStats movementStats;
    public CombatStats combatStats;
    public HealthStats healthStats;

    public InventoryStats inventoryStats;

    public MaterialType materialType;

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
    public bool hasMelee;
    public float meleeDamage;
    public GameObject projectile;
}

[System.Serializable]
public class HealthStats
{
    // Config Stats
    public float maxHealth;
    public float healthRegenerationRate;

    // Starting Stats
    public float health;
    public float armor;
    
    // State Stats
    public bool isAlive;

    public float TakeDamage(float damage)
    {
        float currentHealth = health;
        float damageToTake = damage;
        health = Mathf.Max(0, currentHealth - damageToTake);
        armor = Mathf.Max(0, armor - damageToTake);
        if (health <= 0)
        {
            health = 0;
            armor = 0;
            return -1;
        }

        return health;
    }
}

[CustomEditor(typeof(EntityStats))]
public class EnemyStatsEditor : Editor
{
    SerializedProperty movementStats;
    SerializedProperty combatStats;
    SerializedProperty healthStats;
    SerializedProperty inventoryStats;
    SerializedProperty materialType;


    private void OnEnable()
    {
        movementStats = serializedObject.FindProperty("movementStats");
        combatStats = serializedObject.FindProperty("combatStats");
        healthStats = serializedObject.FindProperty("healthStats");
        inventoryStats = serializedObject.FindProperty("inventoryStats");
        materialType = serializedObject.FindProperty("materialType");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(movementStats);
        EditorGUILayout.PropertyField(combatStats);
        EditorGUILayout.PropertyField(healthStats);
        EditorGUILayout.PropertyField(inventoryStats);
        EditorGUILayout.PropertyField(materialType);

        serializedObject.ApplyModifiedProperties();
    }
}
