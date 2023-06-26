using UnityEngine;

public class SpaceshipManager
{
    private GameObject spaceshipPrefab;

    public SpaceshipManager(GameObject spaceshipPrefab)
    {
        this.spaceshipPrefab = spaceshipPrefab;
    }

    public void DockSpaceship()
    {
        GameObject spaceship = GameObject.Instantiate(spaceshipPrefab);
        // Set spaceship position and other properties as needed
    }
}
