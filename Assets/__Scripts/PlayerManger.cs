using UnityEngine;

public class PlayerManager
{
    private GameObject playerPrefab;

    public PlayerManager(GameObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
    }

    public void SpawnPlayers(int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            GameObject player = GameObject.Instantiate(playerPrefab);
            // Set player position and other properties as needed
        }
    }
}
