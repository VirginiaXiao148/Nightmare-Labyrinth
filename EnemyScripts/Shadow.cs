using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    private Light lightSource; // Referencia a la fuente de luz

    void Start()
    {
        lightSource = GameObject.FindWithTag("Light").GetComponent<Light>(); // Encuentra la fuente de luz en el escenario
    }

    void Update()
    {
        if (lightSource != null && lightSource.intensity > 0)
        {
            Destroy(gameObject); // Destruye la sombra si est� en una zona de luz
        }
        else
        {
            // Seguir al jugador
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // Implementa el c�digo para seguir al jugador aqu�
        Transform player = GameObject.FindWithTag("Player").transform;
        float step = 2f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);
    }
}
