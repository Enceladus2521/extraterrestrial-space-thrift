using UnityEngine;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI highScoreText;

    private void Start()
    {
        UpdateHighScoreText();
    }

    private void UpdateHighScoreText()
    {
        if (GameManager.Instance != null)
        {
            int highScore = GameManager.Instance.GetHighestScore();
            if (highScore == -1) highScoreText.text = "Welcome!"; 
            else highScoreText.text = "" + highScore.ToString();
        }
        else
        {
            Debug.LogWarning("GameManager is missing or not initialized.");
        }
    }
}
