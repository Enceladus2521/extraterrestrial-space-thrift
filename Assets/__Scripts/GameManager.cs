using UnityEngine;

public class GameManager : MonoBehaviour
{
    

    private void Start()
    {
        Application.targetFrameRate = 60;


        

       
    }

    private void Update()
    {
        //if Escape is pressed, quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
    }
}
