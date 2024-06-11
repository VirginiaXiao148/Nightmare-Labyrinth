using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController1 : MonoBehaviour
{
    // El jugador que la cámara debe seguir
    public Transform player;

    // Offset de la cámara desde el jugador
    public Vector3 offset;

    // Velocidad de seguimiento de la cámara
    public float smoothSpeed = 0.125f;

    // Sensibilidad del ratón para rotar la cámara
    public float mouseSensitivity = 100f;

    // Límites de la rotación vertical para evitar que la cámara gire completamente alrededor
    public float minYAngle = -30f;
    public float maxYAngle = 60f;

    // Rotación acumulada en los ejes X e Y
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    void Start()
    {
        // Bloquear el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Obtener la entrada del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ajustar la rotación horizontal
        horizontalRotation += mouseX;

        // Ajustar la rotación vertical y clamping para evitar giros completos
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minYAngle, maxYAngle);

        // Aplicar las rotaciones acumuladas a la cámara
        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
    }

    void LateUpdate()
    {
        offset = new Vector3(0, 0.5f, 0.5f);

        // Calcular la posición deseada de la cámara
        Vector3 desiredPosition = player.position + transform.rotation * offset;

        // Ajustar la posición de la cámara con una interpolación suave
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Asegurarse de que la cámara siempre mire al jugador
        transform.LookAt(player.position + Vector3.up * offset.y);

        // Hacer que el jugador mire en la dirección de la cámara (solo en el eje Y)
        Vector3 lookDirection = transform.forward;
        lookDirection.y = 0; // Mantener la dirección horizontal
        if (lookDirection != Vector3.zero)
        {
            player.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}