using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCloseEntity : MonoBehaviour
{
    [SerializeField]
    Vector3 offsetRotation;

    List<MeshRenderer> meshRenderers;

    // Get all mesh renderers in the child objects except for the current GameObject
    void Start()
    {
        // Get all mesh renderers in the child objects except for the current GameObject
        meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        meshRenderers.Remove(GetComponent<MeshRenderer>());
    }

    void Update()
    {
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
                // Ignore the current GameObject itself
                if (entity.gameObject != gameObject)
                {
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
