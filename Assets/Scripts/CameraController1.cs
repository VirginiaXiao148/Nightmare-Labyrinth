using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    public GameObject player; // Referencia al objeto del jugador

    public float distance = 0.3f; // Distancia entre la cámara y el jugador (reducida para una vista más cercana)
    public float minDistance = 0.2f; // Distancia mínima desde el jugador
    public float maxDistance = 0.4f; // Distancia máxima desde el jugador

    public float rotationSpeed = 5f; // Velocidad de rotación de la cámara
    public float speedH = 2f; // Velocidad de rotación horizontal

    private float yaw;   // Ángulo de rotación horizontal
    private float pitch = 50f; // Ángulo de rotación vertical, inicializado a 50 grados

    public float smoothTime = 0.1f; // Tiempo de suavizado para el movimiento de la cámara
    private Vector3 velocity = Vector3.zero; // Velocidad utilizada para el suavizado

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor en el centro de la pantalla
        Cursor.visible = false; // Oculta el cursor para un control de cámara más inmersivo

        // Inicializa los ángulos de rotación
        yaw = player.transform.eulerAngles.y;

        // Establece la posición inicial de la cámara
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked; // Vuelve a bloquear el cursor si se ha desbloqueado
            Cursor.visible = false; // Asegúrate de que el cursor permanezca invisible
        }

        // Rota la cámara en función del movimiento del ratón
        yaw += speedH * Input.GetAxis("Mouse X");

        // Actualiza la posición de la cámara en relación con el jugador
        UpdateCameraPosition();
    }

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
