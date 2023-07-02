using UnityEngine;
using System.Collections.Generic;

public class LevelScaler
{
public static int GetNumberOfEnemiesToAdd(int difficulty, int currentEnemies)
{
    // Calculate the base number of enemies based on difficulty
    int baseEnemies = Mathf.Clamp(Mathf.FloorToInt(Mathf.Pow(difficulty, 0.2f)) + 2, 1, 50);

    // Introduce randomness and scaling based on current enemies
    int scaledEnemies = Mathf.Clamp(baseEnemies + Random.Range(-2, 3), 1, 50);

    // Increase enemy count if the current number of enemies is low
    int enemiesToAdd = Mathf.Max(scaledEnemies, currentEnemies / 2);

    return enemiesToAdd;
}



public static int GetNumberOfInteractablesToAdd(float difficulty, int currentInteractables)
{
    // Calculate the period and amplitude based on difficulty and player score
    float period = 4f + 2f * Mathf.Sin(difficulty * 0.2f); // Adjustable sine period (start with 4, decrease amplitude over time)
    float amplitude = 2f - 1.5f * Mathf.Sin(difficulty * 0.2f); // Adjustable sine amplitude (start with 2, decrease amplitude over time)

    // Introduce scaling based on current interactables and player score
    float scaleValue = 1f + (amplitude * Mathf.Sin(difficulty / 100f)) + (currentInteractables * 0.2f);

    int interactablesToAdd = Mathf.FloorToInt(scaleValue); // Convert the scale value to an integer for the number of interactables to add

    if (interactablesToAdd < 1)
        interactablesToAdd = 1;

    return interactablesToAdd;
}

}