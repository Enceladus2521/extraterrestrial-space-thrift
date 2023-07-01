using UnityEngine;
using System.Collections.Generic;


public class EntityController : MonoBehaviour
{
    private GameObject closestPlayer;
    private EntityState state;
    private EntityMovementController movementController;
    private EntityCombatController combatController;

    private HealthStats healthStats;
    private InventoryStats inventoryStats;

    private void Start()
    {
        healthStats = state.healthStats;

        CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        // state.LoadPrefab(prefab);

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
        if (gameObject.GetComponent<EntityMovementController>())
            movementController = gameObject.GetComponent<EntityMovementController>();
        else
            movementController = gameObject.AddComponent<EntityMovementController>();

        if (gameObject.GetComponent<EntityCombatController>())
            combatController = gameObject.GetComponent<EntityCombatController>();
        else
            combatController = gameObject.AddComponent<EntityCombatController>();
        
        movementController.UpdateStates(state.movementStats);
        combatController.stats = state.combatStats; // TODO: change to priv
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
        healthStats.TakeDamage(damage);
        if (state != null && healthStats.Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void ApplyRoomEffect(EntityState newState)
    {
        state = newState;
        if (healthStats != null)
            healthStats.ResetEntity();
    }

    public GameObject GetClosestPlayer()
    {
        return closestPlayer;
    }
}