using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Entity Stats")]
public class EntityStats : ScriptableObject
{
    public float speed;
    public bool canFly;
    public float flyingAltitude;
    public bool canShoot;
    public float shootingDamage;
    public bool hasMelee;
    public float meleeDamage;
    public bool maintainsDistance;
    public float distance;
}


[CustomEditor(typeof(EntityStats))]
public class EntityStatsEditor : Editor
{
    SerializedProperty canFly;
    SerializedProperty flyingAltitude;
    SerializedProperty canShoot;
    SerializedProperty shootingDamage;
    SerializedProperty hasMelee;
    SerializedProperty meleeDamage;
    SerializedProperty maintainsDistance;
    SerializedProperty distance;

    private void OnEnable()
    {
        canFly = serializedObject.FindProperty("canFly");
        flyingAltitude = serializedObject.FindProperty("flyingAltitude");
        canShoot = serializedObject.FindProperty("canShoot");
        shootingDamage = serializedObject.FindProperty("shootingDamage");
        hasMelee = serializedObject.FindProperty("hasMelee");
        meleeDamage = serializedObject.FindProperty("meleeDamage");
        maintainsDistance = serializedObject.FindProperty("maintainsDistance");
        distance = serializedObject.FindProperty("distance");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(canFly);
        if (canFly.boolValue)
        {
            EditorGUILayout.PropertyField(flyingAltitude);
        }

        EditorGUILayout.PropertyField(canShoot);
        if (canShoot.boolValue)
        {
            EditorGUILayout.PropertyField(shootingDamage);
        }

        EditorGUILayout.PropertyField(hasMelee);
        if (hasMelee.boolValue)
        {
            EditorGUILayout.PropertyField(meleeDamage);
        }

        EditorGUILayout.PropertyField(maintainsDistance);
        if (maintainsDistance.boolValue)
        {
            EditorGUILayout.PropertyField(distance);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
