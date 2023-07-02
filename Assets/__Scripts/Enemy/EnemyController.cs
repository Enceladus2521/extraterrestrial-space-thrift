
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// This script handles the enemy stats movement and attack
/// </summary>
public class EnemyController : MonoBehaviour
{

    #region Variables
    private GameObject closestPlayer;
    private Rigidbody rb;
    private int attackPointIndex = 0;
    private int difficulty = 0;




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
    public float health = 100f; //can be random



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


    #endregion


    /// <summary>
    /// This function is used setup the enemys components
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        StartCoroutine(UpdateTargetCoroutine());
        if ((EnemyType)enemyType == EnemyType.Eplode) attackRange = 0f;
    }


    /// <summary>
    /// This function is used to generate a random some random stats for the enemy based on the difficulty and seed
    /// </summary>
    public void GenerateRandom(int difficultySeed, int difficulty)
    {
        UnityEngine.Random.InitState(difficultySeed); //set seed for randomizer

        if (enemyType == EnemyType.Eplode) attackRange = 0f; //if enemy is explode set attack range to 0
        health = UnityEngine.Random.Range(1, Random.Range(1 + difficulty * 2, 1 + difficulty * 5)); //set health based on difficulty
        attackRate = UnityEngine.Random.Range(0.5f * (difficulty + 1), 3f); //set attack rate based on difficulty
        int mindamage = (int)(difficulty * 1.01f * 10); //set min damage based on difficulty
        int maxdamage = (int)(difficulty * 1.02f * 10); //set max damage based on difficulty
        //clamp damage between 10 and 30
        if (mindamage < 10) mindamage = 5;
        if (maxdamage > 100) maxdamage = 30;
        attackDamage = UnityEngine.Random.Range(mindamage, maxdamage);

        knockback = UnityEngine.Random.Range(10, difficulty);
        //cap knockback at 0 because it causes bugs
        if (knockback > 0) knockback = 0;
        despawnOnImpact = Random.Range(0, 1) == 0 ? true : false; //set despawn on impact based on random
        projectileForce = UnityEngine.Random.Range(10, difficulty * 1.5f); //set projectile force based on difficulty
        // cap burstAmount at 5
        burstAmount = UnityEngine.Random.Range(1, 5); //set burst amount based on random
        burstRate = UnityEngine.Random.Range(0.1f, 0.3f); //set burst rate based on random
        projectileColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0, 1f)); //set projectile color based on random
        explodeRange = UnityEngine.Random.Range(3, 5); //set explode range based on random

        this.difficulty = difficulty; //set difficulty
    }

    /// <summary>
    /// in The update function the enemy will check if there is a player in range and if so it will attack or follow the player
    /// </summary>
    private void Update()
    {
        if (closestPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, closestPlayer.transform.position);

            if (distance < folowRange && distance > attackRange) //pursue player state
            {
                Rotate();
                Move(closestPlayer);

                currentState = EnemyState.Walking;
            }
            else if (distance < attackRange) //attack player state
            {
                Rotate();

                if (canAttack) Attack();

                currentState = EnemyState.Attacking;
            }
            else // idle state
            {
                currentState = EnemyState.Idle;
            }
        }
    }

    /// <summary>
    /// This function is used to Attack the player
    /// </summary>
    private void Attack()
    {
        StartCoroutine(AttackCoolDown()); //start attack cooldown
        if (enemyType == EnemyType.Projectile) StartCoroutine(SpawnBurst()); //if enemy is projectile spawn a burst of projectiles
        else if (enemyType == EnemyType.Eplode) Explode(); //if enemy is explode explode


    }

    IEnumerator AttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackRate); //wait for attack rate
        canAttack = true;
    }



    #region  Attack

    private void SpawnProjectile()
    {
        GameObject variableForPrefab = Resources.Load("PF_Projectile") as GameObject; //load projectile prefab

        GameObject projectile = Instantiate(variableForPrefab, attackPoints[attackPointIndex].position, attackPoints[attackPointIndex].rotation); //spawn projectile
        projectile.GetComponent<Renderer>().material.color = projectileColor; //set projectile color
        projectile.GetComponent<Renderer>().material.SetColor("_EmissionColor", projectileColor); //set projectile color
        projectile.transform.GetChild(0).GetComponent<TrailRenderer>().startColor = projectileColor; //set projectile color
        projectile.transform.GetChild(0).GetComponent<TrailRenderer>().endColor = projectileColor; // set projectile color


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
        explode.GetComponent<Explosion>().SetExplosionData(attackDamage, knockback, 0, 0, explodeRange, 0.75f, true);
        Destroy(gameObject);

    }


    #endregion


    #region  Damage

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject, 0.1f);
            Die();

        }
    }



    private void Die()
    {
        DropAmmo();
        MoneyDrop();
        DropXp();
    }

    private void DropXp()
    {
        GameObject variableForPrefab = Resources.Load("PF_XP") as GameObject;

        //drop 1-4
        int xpAmount = Random.Range(1, 4);
        for (int i = 0; i < xpAmount; i++)
        {
            GameObject xp = Instantiate(variableForPrefab, transform.position, Quaternion.identity);
            //add rigidbody if not already there
            if (xp.GetComponent<Rigidbody>() == null)
            {
                xp.AddComponent<Rigidbody>();
            }
            xp.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)) * 2, ForceMode.Impulse);
            xp.GetComponent<XPToken>().SetAmount(Mathf.Pow(difficulty, 2) * 10);
        }
    }



    private void DropAmmo()
    {
        GameObject variableForPrefab = Resources.Load("PF_Ammo") as GameObject;

        //drop 1-4
        int ammoAmount = Random.Range(1, 4);
        for (int i = 0; i < ammoAmount; i++)
        {
            GameObject ammo = Instantiate(variableForPrefab, transform.position, Quaternion.identity);
            //add rigidbody if not already there
            if (ammo.GetComponent<Rigidbody>() == null)
            {
                ammo.AddComponent<Rigidbody>();
            }
            ammo.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)) * 2, ForceMode.Impulse);
            ammo.GetComponent<PickUpInteraction>().SetAmount(difficulty + 1 * 10, 0, 0, 0, 1.5f);
        }
    }

    int[] moneyDenominations = { 10, 10, 20, 20, 50, 50, 50, 50, 100, 100, 500, 500 };
    int maxSpawnCount = 5;
    private void MoneyDrop()
    {
        for (int i = 0; i < maxSpawnCount; i++)
        {
            int randomIndex = (int)(Random.value * moneyDenominations.Length);
            int moneyValue = moneyDenominations[randomIndex];
            spawnMoney(moneyValue);
        }
    }




    private void spawnMoney(int value)
    {
        GameObject money;
        int wichOne = Random.Range(1, 2);
        switch (value)
        {
            case 10:
                if (wichOne == 1)
                {
                    money = Resources.Load("PF_Money_10_01") as GameObject;
                }
                else
                {
                    money = Resources.Load("PF_Money_10_02") as GameObject;
                }
                break;
            case 20:
                if (wichOne == 1)
                {
                    money = Resources.Load("PF_Money_20_01") as GameObject;
                }
                else
                {
                    money = Resources.Load("PF_Money_20_02") as GameObject;
                }
                break;
            case 50:
                if (wichOne == 1)
                {
                    money = Resources.Load("PF_Money_50_01") as GameObject;
                }
                else
                {
                    money = Resources.Load("PF_Money_50_02") as GameObject;
                }
                break;
            case 100:
                if (wichOne == 1)
                {
                    money = Resources.Load("PF_Money_100_01") as GameObject;
                }
                else
                {
                    money = Resources.Load("PF_Money_100_02") as GameObject;
                }
                break;
            case 500:
                if (wichOne == 1)
                {
                    money = Resources.Load("PF_Money_500_01") as GameObject;
                }
                else
                {
                    money = Resources.Load("PF_Money_500_02") as GameObject;
                }
                break;
            default:
                if (wichOne == 1)
                {
                    money = Resources.Load("PF_Money_10_01") as GameObject;
                }
                else
                {
                    money = Resources.Load("PF_Money_10_02") as GameObject;
                }
                break;
        }
        if (money != null)
        {
            GameObject newMoney = Instantiate(money, transform.position, Quaternion.identity);
            //set position +2 on y axis
            newMoney.transform.position = new Vector3(newMoney.transform.position.x, newMoney.transform.position.y + 2, newMoney.transform.position.z);

            //give random force to money
            newMoney.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)) * 2, ForceMode.Impulse);
            newMoney.AddComponent<AutoDespawn>().SetTimeToDespawn(40f);
        }



    }




    #endregion




    #region  Movement
    private void Rotate()
    {
        if (closestPlayer != null)
        {

            Vector3 direction = closestPlayer.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

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
            yield return new WaitForSeconds(5f);
        }
    }

    private void UpdateTarget()
    {
        List<GameObject> players = GameManager.Instance?.Players;
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
