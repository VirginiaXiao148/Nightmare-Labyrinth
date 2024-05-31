using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject

    public float distance = 2.0f; // Distance between the camera and the player

    public float rotationSpeed = 5f; // Speed at which the camera rotates

    public float speedH = 2; // Horizontal rotation speed
    public float speedV = 2; // Vertical rotation speed

    float yaw;   // Horizontal rotation angle
    float pitch; // Vertical rotation angle

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor in the center of the screen
        Cursor.visible = false; // Hides the cursor for more immersive camera control

        // Initialize the rotation and position of the camera
        yaw = player.transform.eulerAngles.y; // Set yaw to player's y rotation
        pitch = player.transform.eulerAngles.x; // Set pitch to player's x rotation

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
        pitch -= speedV * Input.GetAxis("Mouse Y");

        // Clamp the pitch to prevent camera flip over
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        // Update the camera position relative to the player
        UpdateCameraPosition();
        RotateCamera();
    }

    void RotateCamera()
    {
        // Get horizontal input for player rotation
        float rotationInput = Input.GetAxis("Horizontal");

        // Calculate rotation angle based on input and rotation speed
        float rotationAngle = rotationInput * rotationSpeed * Time.deltaTime;

        // Rotate the player
        player.transform.rotation = Quaternion.Euler(0, yaw, 0);

        // Optionally, rotate the camera around the player
        // transform.RotateAround(player.transform.position, Vector3.up, rotationAngle);
    }

    void UpdateCameraPosition()
    {
        // Calculate the camera position using spherical coordinates
        Vector3 offset = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = player.transform.position + rotation * offset;

        // Ensure the camera always faces the player
        transform.LookAt(player.transform.position);
    }

    void StartPoint()
    {
        // Set the camera's initial position to the player's position
        transform.position = player.transform.position;
    }
}