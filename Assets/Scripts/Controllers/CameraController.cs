using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject

    public float distance = 0.5f; // Vector representing the distance between the camera and the player

    public float rotationSpeed = 5f; // Speed at which the camera rotates

    public float speedH = 2;
    public float speedV = 2;

    float yaw;
    float pitch;

    // Start is called before the first frame update
    void Start()
    {
        // Bloquea el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        // Oculta el cursor
        Cursor.visible = true;

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

        player.transform.rotation = Quaternion.Euler(0, yaw, 0);

        // Rotate the camera around the player
        UpdateCameraPosition();
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
