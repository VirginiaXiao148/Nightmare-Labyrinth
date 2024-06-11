using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject

    // El jugador que la cámara debe seguir
    public Transform player;

    // Offset de la cámara desde el jugador
    public Vector3 offset;

    // Velocidad de seguimiento de la cámara
    public float smoothSpeed = 0.125f;

    float yaw;
    float pitch;

    // Start is called before the first frame update
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor in the center of the screen
        Cursor.visible = false; // Hides the cursor for a more immersive camera control

        // Inicializa la rotación y posición de la cámara
        yaw = player.transform.eulerAngles.y;
        
        UpdateCameraPosition();
    }

    void FixedUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Posición deseada de la cámara
        Vector3 desiredPosition = player.position + offset;

        // Interpolación suave entre la posición actual y la posición deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Asignar la posición suavizada a la cámara
        transform.position = smoothedPosition;

        // (Opcional) Asegurarse de que la cámara siempre mire al jugador
        // transform.LookAt(player);
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // Rotate the camera
        yaw += speedH * Input.GetAxis("Mouse X");

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

    /* void UpdateCameraPosition()
    {
        // Calcular la posición de la cámara usando coordenadas esféricas
        Vector3 offset = new Vector3(0, 0.5f, -distance);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = player.transform.position + rotation * offset;

        // Asegurarse de que la cámara siempre mire hacia el jugador
        transform.LookAt(player.transform.position);
    } */

    void UpdateCameraPosition()
    {
        // Calcula la posición deseada de la cámara
        Vector3 desiredPosition = CalculateCameraPosition();

        // Realiza la detección de colisiones para ajustar la posición de la cámara
        Vector3 adjustedPosition = CheckForCollisions(desiredPosition);

        // Usa smooth damp para mover la cámara de manera suave
        transform.position = Vector3.SmoothDamp(transform.position, adjustedPosition, ref velocity, smoothTime);

        // Asegúrate de que la cámara siempre mire al jugador
        transform.LookAt(player.transform.position + Vector3.up * 1.5f); // Ajusta el objetivo LookAt para enfocar en la parte superior del cuerpo del jugador
    }

    Vector3 CalculateCameraPosition()
    {
        // Calcula la posición de la cámara usando coordenadas esféricas
        Vector3 offset = new Vector3(0, 0.5f, -distance); // Ajusta el offset de altura para mirar sobre el hombro del jugador
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        return player.transform.position + rotation * offset;
    }

    Vector3 CheckForCollisions(Vector3 targetPosition)
    {
        RaycastHit hit;
        if (Physics.Linecast(player.transform.position, targetPosition, out hit))
        {
            float adjustedDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            Vector3 offset = new Vector3(0, 0.5f, -adjustedDistance); // Ajusta el offset de altura para mirar sobre el hombro del jugador
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            return player.transform.position + rotation * offset;
        }
        return targetPosition;
    }
}