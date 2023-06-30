using UnityEngine;
using System.Collections.Generic;


public class EntityController : MonoBehaviour
{
    public GameObject closestPlayer;
    public EntityPrefab prefab;

    public EntityState state;
    private EntityMovementController movementController;
    private EntityCombatController combatController;

    private void Start()
    {
        if (state == null)
        {
            if (gameObject.GetComponent<EntityState>() != null)
                state = gameObject.GetComponent<EntityState>();
            else
                state = gameObject.AddComponent<EntityState>();
        }

        CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        state.LoadPrefab(prefab);
        state.healthStats.ResetEntity();

        // Fetch the Rigidbody from the GameObject with this script attached
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Stop the Rigidbody from rotating that its not tipping over 
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        InitializeControllers();
    }

    private void InitializeControllers()
    {
        movementController = new EntityMovementController(this, state.movementStats);
        if (gameObject.GetComponent<EntityCombatController>())
            combatController = gameObject.GetComponent<EntityCombatController>();
        else
            combatController = gameObject.AddComponent<EntityCombatController>();
        combatController.stats = state.combatStats;
    }

    public void UpdateTarget()
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
                    // check if is in view distance
                    // if(state.movementStats.viewRange < distance) {
                    //     continue;
                    // }

                    closestDistance = distance;
                    closestPlayer = player;
                    Debug.Log("Closest player: " + closestPlayer);
                    movementController.UpdateTarget(player);
                    combatController.Update();
                }
            }
        }
    }

    void FixedUpdate()
    {
        // if (combatController != null)
        //     combatController.FixedUpdate();
        if (movementController != null)
            movementController.FixedUpdate();
    }

    void Update()
    {
        UpdateTarget();
    }

    public void TakeDamage(float damage)
    {
        state.healthStats.TakeDamage(damage);
        if (state != null && state.healthStats.Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}