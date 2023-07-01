using UnityEngine;

public class EntityMovementController : MonoBehaviour
{
    [SerializeField]
    private MovementStats stats;
    private Rigidbody rb;
    private Transform target;
    private CapsuleCollider capsuleCollider;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        AdjustColliderHeight(stats.flyingAltitude);
    }

    public void UpdateStates(MovementStats stats)
    {
        this.stats = stats;
    }

    public void UpdateTarget(GameObject target)
    {
        this.target = target.transform;
    }

    public void FixedUpdate()
    {
        if (target)
        {
            if (stats.flyingAltitude > 0f)
                Fly();

            float currentDistance = Vector3.Distance(rb.position, target.position);
            MoveToTarget(currentDistance);
        }
    }

    private void MoveToTarget(float currentDistance)
    {
        if (stats.maintainDistance > 0f && currentDistance <= stats.maintainDistance)
            return;

        Vector3 moveDirection = target.position - rb.position;
        moveDirection.y = 0f; // Ignore vertical movement

        if (stats.isStunned)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            Vector3 velocity = rb.velocity;

            if (stats.accelerationDirection == AccelerationDirection.TargetDirection)
            {
                Vector3 targetDirection = target.position - rb.position;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, stats.turnSpeed * Time.deltaTime));
                rb.velocity = rb.velocity + moveDirection.normalized * stats.speed;
            }
            else if (stats.accelerationDirection == AccelerationDirection.CurrentDirection)
            {
                velocity = rb.velocity + moveDirection.normalized * stats.speed;
            }
            else
            {
                velocity = Vector3.zero;
            }

            rb.velocity = velocity * stats.accelerationMultiplier;
        }
    }

    private void Fly()
    {
        Vector3 correctHeight = new Vector3(rb.position.x, stats.flyingAltitude, rb.position.z);
        Vector3 moveDirection = Vector3.MoveTowards(rb.position, correctHeight, Time.deltaTime * stats.speed);
        rb.MovePosition(moveDirection);
    }

    private void AdjustColliderHeight(float multiplier)
    {
        if (capsuleCollider != null)
        {
            float desiredHeight = stats.flyingAltitude;
            capsuleCollider.height = desiredHeight * 2f;
            capsuleCollider.center = new Vector3(0f, stats.flyingAltitude / 2f, 0f);
        }
    }

    // Visualize the view range and view angle in the Scene view
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void OnDrawGizmos()
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