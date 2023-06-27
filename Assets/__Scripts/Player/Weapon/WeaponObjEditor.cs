using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponObj))]
public class WeaponObjEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var weaponObj = target as WeaponObj;

        //header
        GUILayout.Label("-----------------------Weapon Stats-----------------------", EditorStyles.boldLabel);
        weaponObj.weaponType = (WeaponObj.WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponObj.weaponType);

        //weapon info
        weaponObj.weaponName = EditorGUILayout.TextField("Weapon Name", weaponObj.weaponName);
        weaponObj.weaponPrefab = (GameObject)EditorGUILayout.ObjectField("Weapon Prefab", weaponObj.weaponPrefab, typeof(GameObject), false);

        //weapon offset
        weaponObj.weaponOffset = (WeaponObj.WeaponOffset)EditorGUILayout.EnumPopup("Weapon Offset", weaponObj.weaponOffset);

        //display if not default
        if (weaponObj.weaponOffset == WeaponObj.WeaponOffset.customWeaponOffset)
        {
            weaponObj.customPositionOffset = EditorGUILayout.Vector3Field("Custom Position Offset", weaponObj.customPositionOffset);
            weaponObj.customRotationOffset = EditorGUILayout.Vector3Field("Custom Rotation Offset", weaponObj.customRotationOffset);
            weaponObj.customScaleOffset = EditorGUILayout.Vector3Field("Custom Scale Offset", weaponObj.customScaleOffset);
        }
        else
        {
            weaponObj.defaultPositionOffset = EditorGUILayout.Vector3Field("Default Position Offset", weaponObj.defaultPositionOffset);
            weaponObj.defaultRotationOffset = EditorGUILayout.Vector3Field("Default Rotation Offset", weaponObj.defaultRotationOffset);
            weaponObj.defaultScaleOffset = EditorGUILayout.Vector3Field("Default Scale Offset", weaponObj.defaultScaleOffset);
        }

        //weapon stats        
        GUILayout.Label("-----------------------Weapon Behaviour-----------------------", EditorStyles.boldLabel);
        weaponObj.FullAuto = EditorGUILayout.Toggle("Full Auto", weaponObj.FullAuto);
        weaponObj.autoReload = EditorGUILayout.Toggle("Auto Reload", weaponObj.autoReload);
        weaponObj.weaponFireRate = EditorGUILayout.FloatField("Weapon Fire Rate", weaponObj.weaponFireRate);
        weaponObj.projectilesPerShot = EditorGUILayout.IntField("Projectiles Per Shot", weaponObj.projectilesPerShot);

        weaponObj.weaponDamage = EditorGUILayout.IntField("Weapon Damage", weaponObj.weaponDamage);
        weaponObj.weaponRange = EditorGUILayout.FloatField("Weapon Range", weaponObj.weaponRange);

        //weapon reload
        GUILayout.Label("-----------------------Weapon Reload-----------------------", EditorStyles.boldLabel);
        weaponObj.weaponReloadTime = EditorGUILayout.FloatField("Weapon Reload Time", weaponObj.weaponReloadTime);
        weaponObj.weaponClipSize = EditorGUILayout.IntField("Weapon Clip Size", weaponObj.weaponClipSize);
        weaponObj.ammoCostPerShot = EditorGUILayout.IntField("Ammo Cost Per Shot", weaponObj.ammoCostPerShot);

        //weapon recoil
        GUILayout.Label("-----------------------Weapon Recoil-----------------------", EditorStyles.boldLabel);
        weaponObj.spreadIncreasePerShot = EditorGUILayout.FloatField("Spread Increase Per Shot", weaponObj.spreadIncreasePerShot);
        weaponObj.minSpread = EditorGUILayout.FloatField("Min Spread", weaponObj.minSpread);
        weaponObj.maxSpread = EditorGUILayout.FloatField("Max Spread", weaponObj.maxSpread);
        weaponObj.recoilRecoveryRate = EditorGUILayout.FloatField("Recoil Recovery Rate", weaponObj.recoilRecoveryRate);
        weaponObj.recoilRecoveryDelay = EditorGUILayout.FloatField("Recoil Recovery Delay", weaponObj.recoilRecoveryDelay);

        //ammo
        GUILayout.Label("-----------------------Ammo-----------------------", EditorStyles.boldLabel);
        weaponObj.ammoPrefab = (GameObject)EditorGUILayout.ObjectField("Ammo Prefab", weaponObj.ammoPrefab, typeof(GameObject), false);
        
        

        weaponObj.muzzleFlashPrefab = (GameObject)EditorGUILayout.ObjectField("Muzzle Flash Prefab", weaponObj.muzzleFlashPrefab, typeof(GameObject), false);
        if (weaponObj.muzzleFlashPrefab != null)
        {
            weaponObj.muzzleFlashScale = EditorGUILayout.Vector3Field("Muzzle Flash Scale", weaponObj.muzzleFlashScale);
        }

        weaponObj.hitEffectPrefab = (GameObject)EditorGUILayout.ObjectField("Hit Effect Prefab", weaponObj.hitEffectPrefab, typeof(GameObject), false);
        if (weaponObj.hitEffectPrefab != null)
        {
            weaponObj.hitEffectScale = EditorGUILayout.Vector3Field("Hit Effect Scale", weaponObj.hitEffectScale);
        }

        //weapon sound
        GUILayout.Label("-----------------------Weapon Sound-----------------------", EditorStyles.boldLabel);
        weaponObj.shootSound = (AudioClip)EditorGUILayout.ObjectField("Shoot Sound", weaponObj.shootSound, typeof(AudioClip), false);
        weaponObj.reloadSound = (AudioClip)EditorGUILayout.ObjectField("Reload Sound", weaponObj.reloadSound, typeof(AudioClip), false);
        weaponObj.emptySound = (AudioClip)EditorGUILayout.ObjectField("Empty Sound", weaponObj.emptySound, typeof(AudioClip), false);

        //Ammo Behaviour
        GUILayout.Label("-----------------------Ammo Behaviour-----------------------", EditorStyles.boldLabel);

        //knock back
        weaponObj.knockbackForce = EditorGUILayout.FloatField("Knock Back Force", weaponObj.knockbackForce);

        //burn damage
        weaponObj.burnDps = EditorGUILayout.FloatField("Burn Damage Per Second", weaponObj.burnDps);
        weaponObj.burnDuration = EditorGUILayout.FloatField("Burn Duration", weaponObj.burnDuration);

        //explosion damage
        GUILayout.Label("-----------------------Explosion-----------------------", EditorStyles.boldLabel);
        weaponObj.explosionPrefab = (GameObject)EditorGUILayout.ObjectField("Explosion Prefab", weaponObj.explosionPrefab, typeof(GameObject), false);
        if (weaponObj.explosionPrefab != null)
        {
            weaponObj.explosionRadius = EditorGUILayout.FloatField("Explosion Radius", weaponObj.explosionRadius);
            weaponObj.explosionDamageFallOff = EditorGUILayout.FloatField("Explosion Damage Fall Off", weaponObj.explosionDamageFallOff);
        }

        if (weaponObj.ammoPrefab == null)
        {
            //raycast specific
            GUILayout.Label("-----------------------Raycast Specific-----------------------", EditorStyles.boldLabel);        
            weaponObj.tracerPrefab = (GameObject)EditorGUILayout.ObjectField("Tracer Prefab", weaponObj.tracerPrefab, typeof(GameObject), false);
           
           if (weaponObj.tracerPrefab != null)
            {
                //tracer material
                weaponObj.tracerColor = (Material)EditorGUILayout.ObjectField("Tracer Material", weaponObj.tracerColor, typeof(Material), false);
            }

            //piercing
            GUILayout.Label("Piercing", EditorStyles.boldLabel);
            weaponObj.piercingAmount = EditorGUILayout.IntField("Piercing Amount", weaponObj.piercingAmount);
            if (weaponObj.piercingAmount > 0)
            {                
                weaponObj.piercingDamageFallOff = EditorGUILayout.FloatField("Piercing Damage Fall Off", weaponObj.piercingDamageFallOff);
            }


           
        }
        else
        {
            //projectile specific
            GUILayout.Label("-----------------------Projectile Specific-----------------------", EditorStyles.boldLabel);
            weaponObj.lifeTime = EditorGUILayout.FloatField("Life Time", weaponObj.lifeTime);
            weaponObj.sticky = EditorGUILayout.Toggle("Sticky", weaponObj.sticky);
            weaponObj.explodeOnImpact = EditorGUILayout.Toggle("Explode On Impact", weaponObj.explodeOnImpact);
            weaponObj.bounceAmount = EditorGUILayout.IntField("Bounce Amount", weaponObj.bounceAmount);
            weaponObj.trailPrefab = (GameObject)EditorGUILayout.ObjectField("Trail Prefab", weaponObj.trailPrefab, typeof(GameObject), false);

            weaponObj.projectileMovementType = (WeaponObj.ProjectileMovementType)EditorGUILayout.EnumPopup("Projectile Movement Type", weaponObj.projectileMovementType);

            if(weaponObj.projectileMovementType == WeaponObj.ProjectileMovementType.ConstantForce)
            {
                weaponObj.constantForce = EditorGUILayout.FloatField("Constant Force", weaponObj.constantForce);
            }
            else if (weaponObj.projectileMovementType == WeaponObj.ProjectileMovementType.ImpulseForce)
            {
                weaponObj.impulseForce = EditorGUILayout.FloatField("Impulse Force", weaponObj.impulseForce);
                weaponObj.gravityMultiplier = EditorGUILayout.FloatField("Projectile Gravity", weaponObj.gravityMultiplier);
            }
            

            
            
            

            
            
        }



    }
}