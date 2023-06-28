using UnityEngine;

public class EntityMovementController
{
    private EntityController entityController;
    private MovementStats stats;
    private Rigidbody rb;
    private Transform target;
    private CapsuleCollider capsuleCollider;

    public EntityMovementController(EntityController entityController, MovementStats stats)
    {
        this.entityController = entityController;
        this.stats = stats;
        rb = entityController.GetComponent<Rigidbody>();
        capsuleCollider = entityController.GetComponent<CapsuleCollider>();
        AdjustColliderHeight();
    }

    public void Update()
    {
        if (target != null)
        {
            // Implement movement logic here based on the stats and target
            if (stats.flyingAltitude > 0f)
            {
                Fly();
            }
            else
            {
                MoveToTarget();
            }

            if (stats.maintainDistance > 0f)
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

    public void FixedUpdate()
    {
        MoveEntity();
    }

    private void MoveEntity()
    {
        if (target == null) return;
        Vector3 moveDirection = target.position - rb.position;
        Vector3 velocity = moveDirection.normalized * stats.speed;
        rb.velocity = velocity;
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
        AdjustColliderHeight();
    }

    private void AdjustColliderHeight()
    {
        if (capsuleCollider != null)
        {
            float desiredHeight = stats.flyingAltitude * 2f;
            capsuleCollider.height = desiredHeight;
            capsuleCollider.center = new Vector3(0f, desiredHeight * 0.5f, 0f);
        }
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
        if (currentDistance < stats.maintainDistance)
        {
            // Move away from the target
            Vector3 moveDirection = (rb.position - target.position).normalized;
            rb.position += moveDirection * (stats.maintainDistance - currentDistance);
        }
    }

    private void FindClosestPlayer()
    {
        Transform closestPlayer = entityController.GetClosestPlayer();
        if (closestPlayer != null)
        {
            target = closestPlayer;
        }
        else
        {
            // Handle the case when the target player is null
            // For example, you can stop moving or perform any necessary actions
            // You can also log a message for debugging purposes
        }
    }

    // Visualize the view range and view angle in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (stats.viewRange > 0f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rb.position, stats.viewRange);
        }

        if (stats.viewAngle > 0f)
        {
            Vector3 leftDir = Quaternion.Euler(0f, -stats.viewAngle * 0.5f, 0f) * rb.transform.forward;
            Vector3 rightDir = Quaternion.Euler(0f, stats.viewAngle * 0.5f, 0f) * rb.transform.forward;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(rb.position, leftDir * stats.viewRange);
            Gizmos.DrawRay(rb.position, rightDir * stats.viewRange);
        }
    }
}
