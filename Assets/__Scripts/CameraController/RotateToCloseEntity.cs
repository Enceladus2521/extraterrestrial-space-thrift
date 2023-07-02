using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCloseEntity : MonoBehaviour
{

    [SerializeField]
    Vector3 offsetRotation;

    // Debug log the name of parent on start
    void Start()
    {
        Debug.Log(gameObject.name);
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
                // Rotate the current object to look at the closest entity
                Vector3 directionToClosest = closestEntity.transform.position - transform.position;
                directionToClosest.y = 0f; // Lock rotation only around the y-axis (horizontal)
                Quaternion targetRotation = Quaternion.LookRotation(directionToClosest, Vector3.up);
                transform.rotation = new Quaternion(offsetRotation.x + targetRotation.x, offsetRotation.y + targetRotation.y, offsetRotation.z + targetRotation.z, 1f);
            }
        }
    }
}
