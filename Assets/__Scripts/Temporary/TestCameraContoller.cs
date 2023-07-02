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

    private Quaternion initialRotation;
    [SerializeField] private float maxTiltAngle = 20f;

    // Start is called before the first frame update
    void Start()
    {
        // Get target
        target = transform.parent.GetComponent<Transform>();

        // Calculate offset
        offset = transform.position - target.position;

        // Save the initial rotation
        initialRotation = transform.rotation;

        // Unparent camera
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        // If target is further than units away
        currentSpeedMulti = Vector3.Distance(transform.position, target.position) > distanceToMultiplySpeed ? speedMultiplier : 1f;

        // Calculate target rotation based on the target transform's z position
        float targetRotationX = 0f; // Initialize the target rotation X to 0

        if (target.position.z < 0f)
        {
            // Player is below 0, tilt should be positive
            targetRotationX = Mathf.Clamp(Mathf.Abs(target.position.z), 0f, maxTiltAngle);
        }
        else if (target.position.z > 0f)
        {
            // Player is above 0, tilt should be negative
            targetRotationX = -Mathf.Clamp(Mathf.Abs(target.position.z), 0f, maxTiltAngle);
        }

        // Calculate the additional vertical movement based on player's position
        float additionalVerticalMovement = Mathf.Clamp(target.position.z, -maxTiltAngle, maxTiltAngle);

        // Update the offset vector to add vertical movement
        Vector3 updatedOffset = offset + Vector3.down * additionalVerticalMovement;

        // This transform stays the same relative position to the target smoothly moves towards it
        transform.position = Vector3.SmoothDamp(transform.position, target.position + updatedOffset, ref velocity, speed * currentSpeedMulti);

        Quaternion targetRotation = initialRotation * Quaternion.Euler(targetRotationX, 0f, 0f);

        // Apply the rotation to the camera
        transform.rotation = targetRotation;
    }



}
