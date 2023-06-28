using UnityEngine;
using System.Collections.Generic;

public class EntityMovementController : MonoBehaviour
{
    public EntityStats stats;
    private Rigidbody rb;
    private Transform target;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        FindClosestPlayer();
    }

    private void Update()
    {
        if (target != null)
        {
            if (stats.canFly)
            {
                Fly();
            }
            else
            {
                MoveToTarget();
            }

            if (stats.maintainsDistance)
            {
                MaintainDistance();
            }

            LookAtTarget();
        }
        else
        {
            FindClosestPlayer();
        }
    }

    private void FixedUpdate()
    {
        MoveEntity();
    }

    private void MoveToTarget()
    {
        Vector3 targetPosition = target.position;
        targetPosition.y = rb.position.y; // Maintain the entity's current y position
        rb.position = targetPosition;
    }

    private void Fly()
    {
        Vector3 newPosition = new Vector3(rb.position.x, stats.flyingAltitude, rb.position.z);
        rb.position = newPosition;
    }

    private void LookAtTarget()
    {
        Vector3 targetDirection = target.position - rb.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        rb.MoveRotation(targetRotation);
    }

    private void MaintainDistance()
    {
        float currentDistance = Vector3.Distance(rb.position, target.position);
        if (currentDistance < stats.distance)
        {
            // Move away from the target
            Vector3 moveDirection = (rb.position - target.position).normalized;
            rb.position += moveDirection * (stats.distance - currentDistance);
        }
    }

    private void MoveEntity()
    {
        if (target == null) return;
        Vector3 moveDirection = target.position - rb.position;
        Vector3 velocity = moveDirection.normalized * stats.speed;
        rb.velocity = velocity;
    }

   private void FindClosestPlayer()
{
    List<GameObject> players = GameManager.Instance?.GameState?.getPlayers();
    if (players != null && players.Count > 0)
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestPlayer = null;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null)
        {
            target = closestPlayer.transform;
        }
    }
}

}
