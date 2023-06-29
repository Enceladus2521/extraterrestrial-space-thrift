using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Enemy Stats")]
public class EntityPrefab : ScriptableObject
{
    public MovementStats movementStats;
    public CombatStats combatStats;
    public HealthStats healthStats;

    public InventoryStats inventoryStats;

    public MaterialType materialType;

}

[CustomEditor(typeof(EntityPrefab))]
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
