using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public Transform player; // Reference to the player Transform
    public float distance = 2.0f; // Distance between the camera and the player
    public float rotationSpeed = 5f; // Speed at which the camera rotates

    private float yaw;   // Horizontal rotation angle
    private float pitch; // Vertical rotation angle

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor in the center of the screen
        Cursor.visible = false; // Hides the cursor for more immersive camera control

        // Initialize the rotation angles
        yaw = player.eulerAngles.y;
        pitch = 0; // Usually start with zero pitch
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        // Rotate the camera based on mouse movement
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // Clamp the pitch to prevent camera flip over
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        // Update the camera position relative to the player
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Calculate the camera position using spherical coordinates
        Vector3 offset = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = player.position + rotation * offset;

        // Ensure the camera always faces the player
        transform.LookAt(player);
    }
}
