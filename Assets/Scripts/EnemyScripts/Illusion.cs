using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illusion : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Destruye la ilusi�n cuando el jugador colisiona con ella
        }
    }
}
