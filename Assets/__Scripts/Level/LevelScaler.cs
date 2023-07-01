using UnityEngine;
using System.Collections.Generic;

public class LevelScaler
{
    public static int GetNumberOfEnemiesToAdd(int difficultyLevel)
    {
        int enemiesToAdd = Mathf.FloorToInt(difficultyLevel / 4f); // Increase enemies by 1 for every 4 difficulty levels
        
        if (enemiesToAdd < 1)
            enemiesToAdd = 1;

        return enemiesToAdd;
    }

    public static int GetNumberOfInteractablesToAdd(float difficultyLevel)
    {
        float period = 4f + 2f * Mathf.Sin(difficultyLevel * 0.2f); // Adjustable sine period (start with 4, decrease amplitude over time)
        float amplitude = 2f - 1.5f * Mathf.Sin(difficultyLevel * 0.2f); // Adjustable sine amplitude (start with 2, decrease amplitude over time)
        float scaleValue = 1f + amplitude * Mathf.Sin(Time.time / period);
        int interactablesToAdd = Mathf.FloorToInt(scaleValue); // Convert the scale value to an integer for the number of interactables to add
        
        if (interactablesToAdd < 1)
            interactablesToAdd = 1;
        return interactablesToAdd;
    }
}