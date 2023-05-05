using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraContoller : MonoBehaviour
{
    private Transform target;

    [SerializeField] private float speed;
    [SerializeField] private float speedMultiplier;

    [SerializeField] private float distanceToMultiplySpeed;
    private float currentSpeedMulti = 1f;

    Vector3 offset;
    Vector3 velocity = Vector3.zero;




    // Start is called before the first frame update
    void Start()
    {
        //get target
        target = transform.parent.GetComponent<Transform>();

        //calculate offset
        offset = transform.position - target.position;


        


        //unparent camera
        transform.parent = null;



    }

    // Update is called once per frame
    void Update()
    {
        //if target is further than units away
        currentSpeedMulti = Vector3.Distance(transform.position, target.position) > distanceToMultiplySpeed ? speedMultiplier : 1f;

         

        //this transform stays same relative position to the target smoothly moves towards it
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, speed);

    }
}
