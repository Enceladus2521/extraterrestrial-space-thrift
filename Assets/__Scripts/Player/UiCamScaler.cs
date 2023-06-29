using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCamScaler : MonoBehaviour
{
    //Get parent camera
    private Camera cam;
    //Get this canvas
    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        //Get parent camera
        cam = GetComponentInParent<Camera>();
        //Get this canvas
        canvas = GetComponent<Canvas>();
    }


    // Update is called once per frame
    void Update()
    {
        //get camera width and height
        float camWidth = cam.orthographicSize * 2 * cam.aspect;
        float camHeight = cam.orthographicSize * 2;

        //set canvas width and height
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(camWidth, camHeight);


        
        
    }
}
