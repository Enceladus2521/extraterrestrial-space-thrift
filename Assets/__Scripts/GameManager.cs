using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject spaceshipPrefab;

    private PlayerManager playerManager;
    private SpaceshipManager spaceshipManager;

    private void Start()
    {
        Application.targetFrameRate = 60;
        playerManager = new PlayerManager(playerPrefab);
        spaceshipManager = new SpaceshipManager(spaceshipPrefab);

        playerManager.SpawnPlayers(2);
        spaceshipManager.DockSpaceship();
    }
}
