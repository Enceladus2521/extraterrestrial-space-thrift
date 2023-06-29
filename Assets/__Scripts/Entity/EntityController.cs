using UnityEngine;
using System.Collections.Generic;

public class EntityController : MonoBehaviour
{
    public EntityPrefab prefab;

    public EntityState state;
    private EntityMovementController movementController;
    private EntityCombatController combatController;

    private void Start()
    {
        // if trhere is no Entity State create a new one
        if (state == null)
        {
            // check if Entity state us already created
            if (gameObject.GetComponent<EntityState>() != null)
                state = gameObject.GetComponent<EntityState>();
            else
                state = gameObject.AddComponent<EntityState>();
        }

        CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        state.LoadPrefab(prefab);
        state.healthStats.ResetEntity();
        InitializeControllers();
    }


    private void InitializeControllers()
    {
        movementController = new EntityMovementController(this, state.movementStats);
        combatController = new EntityCombatController(this, state.combatStats);
    }

    public Transform GetClosestPlayer()
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

            return closestPlayer.transform;
        }

        return null;
    }


    void FixedUpdate()
    {
        if (movementController != null)
            movementController.FixedUpdate();
        if (combatController != null)
            combatController.FixedUpdate();
    }

    void Update()
    {
        if (movementController != null)
            movementController.Update();
        if (combatController != null)
            combatController.Update();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("TakeDamage");
        state.healthStats.TakeDamage(damage);
        if (state != null && state.healthStats.Health <= 0)
        {
            Destroy(gameObject);
        }
    }


}
