using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCloseEntity : MonoBehaviour
{
    [SerializeField]
    Vector3 offsetRotation;

    Transform parent;
    List<MeshRenderer> meshRenderers;

    // Debug log the name of parent on start
    void Start()
    {
        Debug.Log(gameObject.name);
        // Unchild it
        parent = transform.parent;
        transform.parent = null;

        // Get all mesh renderers in the child objects
        meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
    }

    void Update()
    {
        // Get the player's rotation
        transform.position = parent.transform.position;

        // Get all entities in the scene (You may need to modify this based on how entities are represented)
        List<EnemyController> entities = LevelManager.Instance?.Entities;

        if (entities != null && entities.Count > 0)
        {
            // Find the closest entity and rotate to look at it
            EnemyController closestEntity = null;
            float closestDistance = Mathf.Infinity;
            Vector3 position = transform.position;

            for (int i = 0; i < entities.Count; i++)
            {
                EnemyController entity = entities[i];
                // Calculate the distance between the current object and the entity
                Vector3 diff = entity.transform.position - position;
                float curDistance = diff.sqrMagnitude;

                // Check if this entity is closer than the previous closest one
                if (curDistance < closestDistance)
                {
                    closestEntity = entity;
                    closestDistance = curDistance;
                }
            }

            if (closestEntity != null)
            {
                // Calculate the direction to the closest entity
                Vector3 directionToClosest = closestEntity.transform.position - transform.position;
                directionToClosest.y = 0f; // Lock rotation only around the y-axis (horizontal)
                Quaternion targetRotation = Quaternion.LookRotation(directionToClosest, Vector3.up);

                // Combine the rotations with the offset and apply the relative rotation
                Quaternion finalRotation = targetRotation * Quaternion.Euler(offsetRotation);
                transform.rotation = finalRotation;

                // Activate mesh renderers
                SetMeshRenderersActive(true);
            }
            else
            {
                // No entity found, deactivate mesh renderers
                SetMeshRenderersActive(false);
            }
        }
    }

    void SetMeshRenderersActive(bool active)
    {
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = active;
        }
    }
}
