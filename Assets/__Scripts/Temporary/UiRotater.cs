using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiRotater : MonoBehaviour
{

    RectTransform rectTransform;
    float rotationSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the ui
        rectTransform.Rotate(0, 0, rotationSpeed);
        
    }
}
