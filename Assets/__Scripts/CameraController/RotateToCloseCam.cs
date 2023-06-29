using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCloseCam : MonoBehaviour
{
    



    
    


    void Update()
    {
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("Camera");

        
        //find the closest camera and rotate to it        
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject cam in cameras)
        {
            Vector3 diff = cam.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = cam;
                distance = curDistance;
            }
        }

        if (closest != null)
        {
            //rotate away from the camera
            transform.rotation = Quaternion.LookRotation(transform.position - closest.transform.position);
        }


    }
}
