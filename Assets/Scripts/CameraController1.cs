using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject

    public float distance = 2.0f; // Vector representing the distance between the camera and the player

    public float rotationSpeed = 5f; // Speed at which the camera rotates

    public float speedH = 2;
    public float speedV = 2;

    float yaw;
    float pitch;

    // Start is called before the first frame update
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor in the center of the screen
        Cursor.visible = false; // Hides the cursor for a more immersive camera control
        
        // Inicializa la rotación y posición de la cámara
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
        
        UpdateCameraPosition();
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        // Rotate the camera
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        // Clampa el pitch para evitar vuelcos de la cámara
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        // Rotate the camera around the player
        UpdateCameraPosition();
        RotateCamera();
    }

    void RotateCamera()
{
    // Obtener la entrada horizontal para la rotación del jugador
    float rotationInput = Input.GetAxis("Horizontal");

    // Calcular el ángulo de rotación basado en la entrada y la velocidad de rotación
    float rotationAngle = rotationInput * rotationSpeed * Time.deltaTime;

    // Rotar el jugador
    player.transform.rotation = Quaternion.Euler(0, yaw, 0);

    // Rotar la cámara alrededor del jugador si es necesario (opcional)
    // transform.RotateAround(player.transform.position, Vector3.up, rotationAngle);
}

    void UpdateCameraPosition()
    {
        // Calcular la posición de la cámara usando coordenadas esféricas
        Vector3 offset = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = player.transform.position + rotation * offset;

        // Asegurarse de que la cámara siempre mire hacia el jugador
        transform.LookAt(player.transform.position);
    }
}
