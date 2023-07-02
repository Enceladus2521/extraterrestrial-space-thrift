using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float distanceToMultiplySpeed = 5f;
    private float currentSpeedMulti = 1f;

    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private float maxTiltAngle = 20f;
    [SerializeField] private float maxOffsetPosition = 10f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;


    // Start is called before the first frame update
    void Start()
    {
        //get target
        target = transform.parent.GetComponent<Transform>();

        //calculate offset
        offset = transform.position - target.position;

        //unparent camera
        transform.parent = null;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //if target is further than units away
        currentSpeedMulti = Vector3.Distance(transform.position, target.position) > distanceToMultiplySpeed ? speedMultiplier : 1f;



        // Calculate target rotation based on the target transform's z position
        float targetRotationX = 0f; // Initialize the target rotation X to 0

        float targetPositionZ = target.position.z;

        if (target.position.z < 0f)
        {
            // Player is below 0, tilt should be positive
            targetPositionZ = Mathf.Clamp(Mathf.Abs(target.position.z), 0f, maxOffsetPosition) / 2;
            targetRotationX = Mathf.Clamp(Mathf.Abs(target.position.z), 0f, maxTiltAngle);
        }
        else if (target.position.z > 0f)
        {
            // Player is above 0, tilt should be negative
            targetPositionZ = Mathf.Clamp(Mathf.Abs(target.position.z), 0f, maxOffsetPosition) / 2;
            targetRotationX = Mathf.Clamp(Mathf.Abs(target.position.z), 0f, maxTiltAngle);
        }

        Quaternion targetRotation = initialRotation * Quaternion.Euler(targetRotationX, 0f, 0f);


        Vector3 localOffset = new Vector3(0, 0, targetPositionZ);
        //this transform stays same relative position to the target smoothly moves towards it
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset + localOffset, ref velocity, speed);
        // Apply the rotation to the camera
        transform.rotation = targetRotation;

    }
}
