using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllEnemies : MonoBehaviour
{
    public int damageAmount = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Obtener el componente AricController1 del jugador y aplicarle daño
            AricController1 playerController = other.GetComponent<AricController1>();
            if (playerController != null)
            {
                playerController.TakeDamage(damageAmount);
            }
        }
    }
}
