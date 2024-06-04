using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject

    public float distance = 2.0f; // Distance between the camera and the player
    public float minDistance = 1.0f; // Minimum distance from the player
    public float maxDistance = 3.0f; // Maximum distance from the player

    public float rotationSpeed = 5f; // Speed at which the camera rotates

    public float speedH = 2; // Horizontal rotation speed
    public float speedV = 2; // Vertical rotation speed

    float yaw;   // Horizontal rotation angle
    float pitch; // Vertical rotation angle

    public float smoothTime = 0.1f; // Smoothing time for camera movement
    private Vector3 velocity = Vector3.zero; // Velocity used for smoothing

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor in the center of the screen
        Cursor.visible = false; // Hides the cursor for more immersive camera control

        // Initialize the rotation angles
        yaw = player.transform.eulerAngles.y;
        pitch = 0; // Initialize pitch to 0 to avoid any unexpected rotation

        // Set the initial position of the camera
        UpdateCameraPosition();
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked; // Re-lock the cursor if it has been unlocked
            Cursor.visible = false; // Ensure the cursor remains invisible
        }

        // Rotate the camera based on mouse movement
        yaw += speedH * Input.GetAxis("Mouse X");
        // pitch -= speedV * Input.GetAxis("Mouse Y"); // Comment out this line to disable vertical movement

        // Clamp the pitch to prevent camera flip over
        // pitch = Mathf.Clamp(pitch, -89f, 89f); // Comment out this line as well since we're not changing the pitch anymore

        // Update the camera position relative to the player
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Calculate the desired camera position
        Vector3 desiredPosition = CalculateCameraPosition();

        // Perform collision detection to adjust the camera's position
        Vector3 adjustedPosition = CheckForCollisions(desiredPosition);

        // Use smooth damp to move the camera smoothly
        transform.position = Vector3.SmoothDamp(transform.position, adjustedPosition, ref velocity, smoothTime);

        // Ensure the camera always faces the player
        transform.LookAt(player.transform.position + Vector3.up * 0.5f); // Adjust the LookAt target to focus on the player's upper body
    }

    Vector3 CalculateCameraPosition()
    {
        // Calculate the camera position using spherical coordinates
        Vector3 offset = new Vector3(0, 1.5f, -distance); // Adjust the height offset to look over the player's shoulder
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        return player.transform.position + rotation * offset;
    }

    Vector3 CheckForCollisions(Vector3 targetPosition)
    {
        RaycastHit hit;
        if (Physics.Linecast(player.transform.position, targetPosition, out hit))
        {
            float adjustedDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            Vector3 offset = new Vector3(0, 1.5f, -adjustedDistance); // Adjust the height offset to look over the player's shoulder
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            return player.transform.position + rotation * offset;
        }
        return targetPosition;
    }
}