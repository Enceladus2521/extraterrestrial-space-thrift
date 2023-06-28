using UnityEngine;
using System.Collections.Generic;

public class EntityController : MonoBehaviour
{
    public EntityStats entityStats;

    private EntityMovementController movementController;
    private EntityCombatController combatController;

    private void Start()
    {
        CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        
        InitializeControllers();
    }

    private void InitializeControllers()
    {
        movementController = new EntityMovementController(this, entityStats.movementStats);
        combatController = new EntityCombatController(this, entityStats.combatStats);
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

    // on damage
    public void TakeDamage(float damage)
    {
        float currentHealth = entityStats.healthStats.TakeDamage(damage);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        movementController.FixedUpdate();
        combatController.FixedUpdate();
    }

    void Update()
    {
        movementController.Update();
        combatController.Update();
    }

}
