
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{

    private GameObject closestPlayer;
    private Rigidbody rb;
    private int attackPointIndex = 0;




    private bool canAttack = true;

    public EnemyType enemyType; //can be random    
    public enum EnemyType
    {
        Eplode = 0,
        Projectile = 1
    }

    public EnemyState currentState;
    public enum EnemyState
    {
        Idle = 0,
        Walking = 1,
        Attacking = 2,
    }

    [Header("Health Stats")]
    private float health = 100f; //can be random



    [Header("Enemy Stats")]
    [SerializeField] private float speed = 30f;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float folowRange = 20f;

    [Header("Attack Stats--------------------")]

    [SerializeField] private float attackRange = 5f;
    private float attackRate = 1f; //can be random
    private float attackDamage = 10f; //can be random
    private float knockback = 10f; //can be random


    [Header("Projectile Stats")]
    private bool despawnOnImpact = true; //can be random
    private float projectileForce = 15f; //can be random
    [SerializeField] float timeToLive = 5f;
    private int burstAmount = 1; //can be random
    private float burstRate = 0.3f; //can be random 0.2f-0.5f

    private Color projectileColor; //can be random 



    [Header("Eplode Stats")]
    private float explodeRange = 5f; //can be random

    [SerializeField] List<Transform> attackPoints;




    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        StartCoroutine(UpdateTargetCoroutine());
    }

    public void SetEnemyStats(int enemyType, float health, float attackRate, float attackDamage, float knockback, bool despawnOnImpact,
        float projectileForce, int burstAmount, float burstRate, Color projectileColor, float explodeRange)
    {
        this.enemyType = (EnemyType)enemyType;
        if ((EnemyType)enemyType == EnemyType.Eplode) attackRange = 0f;
        this.health = health;
        this.attackRate = attackRate;
        this.attackDamage = attackDamage;
        this.knockback = knockback;
        this.despawnOnImpact = despawnOnImpact;
        this.projectileForce = projectileForce;
        this.burstAmount = burstAmount;
        this.burstRate = burstRate;
        this.projectileColor = projectileColor;
        this.explodeRange = explodeRange;        
    }

    public void GenerateRandom(int difficultySeed, int difficultyLevel)
    {
        UnityEngine.Random.InitState(difficultySeed);
        enemyType = (EnemyType)UnityEngine.Random.Range(0, 1); 
        if (enemyType == EnemyType.Eplode) attackRange = 0f;
        health = UnityEngine.Random.Range(1, Random.Range(1 + difficultyLevel * 2, 1 + difficultyLevel * 5));
        attackRate = UnityEngine.Random.Range(0.5f * difficultyLevel, 5f );
        attackDamage = UnityEngine.Random.Range(10, 10 + difficultyLevel * 2);
        knockback = UnityEngine.Random.Range(10, 10 + difficultyLevel * 2);
        despawnOnImpact = Random.Range(0, 1) == 0 ? true : false;
        projectileForce = UnityEngine.Random.Range(5, 10 + difficultyLevel * 2);
        // cap burstAmount at 5
        burstAmount = UnityEngine.Random.Range(1, 5);
        burstRate = UnityEngine.Random.Range(0.2f, 0.5f);
        projectileColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0, 1f));
        explodeRange = UnityEngine.Random.Range(3, 5);
    }

    private void Update()
    {
        if (closestPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, closestPlayer.transform.position);

            if (distance < folowRange && distance > attackRange)
            {
                Rotate();
                Move(closestPlayer);

                currentState = EnemyState.Walking;
            }
            else if (distance < attackRange)
            {
                Rotate();

                if (canAttack) Attack();

                currentState = EnemyState.Attacking;
            }
            else
            {
                currentState = EnemyState.Idle;
            }
        }
    }


    private void Attack()
    {
        StartCoroutine(AttackCoolDown());
        if (enemyType == EnemyType.Projectile) StartCoroutine(SpawnBurst());

        else if (enemyType == EnemyType.Eplode) Explode();
    }

    IEnumerator AttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackRate);
        canAttack = true;
    }



    #region  Attack

    private void SpawnProjectile()
    {
        GameObject variableForPrefab = Resources.Load("PF_Projectile") as GameObject;

        GameObject projectile = Instantiate(variableForPrefab, attackPoints[attackPointIndex].position, attackPoints[attackPointIndex].rotation);
        projectile.GetComponent<Renderer>().material.color = projectileColor;
        projectile.GetComponent<Renderer>().material.SetColor("_EmissionColor", projectileColor);
        
        projectile.GetComponent<EnemyProjectile>().SetProjectileData(attackDamage, timeToLive, despawnOnImpact, projectileForce, knockback);

        //check if attackPointIndex can be incremented
        if (attackPointIndex < attackPoints.Count - 1)
        {
            attackPointIndex++;
        }
        else
        {
            attackPointIndex = 0;
        }


    }

    IEnumerator SpawnBurst()
    {
        for (int i = 0; i < burstAmount; i++)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(burstRate);
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (enemyType == EnemyType.Eplode)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Explode();
            }
        }
    }

    private void Explode()
    {

        GameObject variableForPrefab = Resources.Load("PF_Explosion") as GameObject;
        GameObject explode = Instantiate(variableForPrefab, transform.position, transform.rotation);
        explode.GetComponent<Explosion>().damagePlayer = true;
        explode.GetComponent<Explosion>().SetExplosionData(attackDamage, knockback, 0, 0, explodeRange, 0.75f);
        Destroy(gameObject);

    }


    #endregion


    #region  Damage

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Todo: Spawn death effect
        //Todo: Spawn loot
        Destroy(gameObject);
    }

    #endregion




    #region  Movement
    private void Rotate()
    {
        if (closestPlayer != null)
        {
            Vector3 targetDirection = closestPlayer.transform.position - transform.position;
            rb.AddTorque(Vector3.Cross(transform.forward, targetDirection) * rotationSpeed * Time.deltaTime);
        }
    }
    private void Move(GameObject player)
    {
        if (player != null)
        {
            rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.Impulse);
        }
    }

    #endregion

    #region GetPlayer

    private bool IsPlayerInWalkDistance()
    {
        if (closestPlayer != null)
        {
            if (Vector3.Distance(transform.position, closestPlayer.transform.position) < folowRange)
            {
                return true;
            }
        }
        return false;
    }
    IEnumerator UpdateTargetCoroutine()
    {
        while (true)
        {
            UpdateTarget();
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateTarget()
    {
        List<GameObject> players = GameManager.Instance?.GameState?.getPlayers();
        if (players != null && players.Count > 0)
        {
            float closestDistance = Mathf.Infinity;
            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }
        }
    }
    #endregion



#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        switch ((int)currentState)
        {
            case 0:
                Gizmos.color = Color.green; // Idle
                Gizmos.DrawWireSphere(transform.position, folowRange);


                break;
            case 1:
                Gizmos.color = Color.yellow; // Walking
                Gizmos.DrawWireSphere(transform.position, folowRange);

                Gizmos.color = Color.red; // Attacking
                Gizmos.DrawWireSphere(transform.position, attackRange);
                if (closestPlayer != null)
                {
                    Gizmos.DrawLine(transform.position, closestPlayer.transform.position);
                }
                break;
            case 2:
                Gizmos.color = Color.red; // Attacking
                Gizmos.DrawWireSphere(transform.position, attackRange);
                if (closestPlayer != null)
                {
                    Gizmos.DrawLine(transform.position, closestPlayer.transform.position);
                }
                break;
        }
    }

#endif

}