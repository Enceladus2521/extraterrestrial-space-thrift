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

        // This transform stays the same relative position to the target smoothly moves towards it
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, speed * currentSpeedMulti);

        // Calculate target rotation based on the target transform's z position
        float tiltAmount = Mathf.Clamp(Mathf.Abs(target.position.z), 0f, maxTiltAngle);
        float targetRotationX = target.position.z < 0f ? tiltAmount : -tiltAmount;

        Quaternion targetRotation = initialRotation * Quaternion.Euler(targetRotationX, 0f, 0f);

        // Apply the rotation to the camera
        transform.rotation = targetRotation;
    }
}
