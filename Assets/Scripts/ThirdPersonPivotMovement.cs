using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPivotMovement : MonoBehaviour
{
    public GameObject player;
    public GameObject cam;
    public float sensitivityX = 10f;
    public float sensitivityY = 10f;
    private float pitch = 0f;

    public float maxCameraDistance = 5f; // Also the default
    public float minCameraDistance = 1.5f;
    public float sphereRadius = 0.1f;
    public float smoothSpeed = 20f;
    private Vector3 cameraOffset;

    public LayerMask collisionLayers; // LayerMask to specify collision layers (assign in the inspector)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraOffset = cam.transform.localPosition;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            doMouseMovement(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        HandleCameraCollision();
    }

    void doMouseMovement(float mInputX, float mInputY)
    {
        pitch -= mInputY * sensitivityY;
        pitch = Mathf.Clamp(pitch, -90f, 65f);

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        player.transform.Rotate(Vector3.up * mInputX * sensitivityX);
    }

    void HandleCameraCollision()
    {
        Vector3 desiredPosition = transform.position + transform.TransformDirection(cameraOffset.normalized * maxCameraDistance);
        RaycastHit hit;

        // Perform SphereCast, ignoring objects in the specified LayerMask
        if (Physics.SphereCast(transform.position, sphereRadius, (desiredPosition - transform.position).normalized, out hit, maxCameraDistance, ~collisionLayers))
        {
            // Adjust camera position based on collision
            desiredPosition = hit.point - (desiredPosition - transform.position).normalized * sphereRadius;
        }

        // Smoothly move the camera to the desired position
        cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        // Rotate the camera to face the player
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - cam.transform.position, Vector3.up);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
