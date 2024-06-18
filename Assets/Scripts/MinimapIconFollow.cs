using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconFollow : MonoBehaviour
{
    public Transform player; // Asignar el transform del jugador

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
