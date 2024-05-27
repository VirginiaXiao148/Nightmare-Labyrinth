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
            Destroy(gameObject); // Destruye la sombra si está en una zona de luz
        }
        else
        {
            // Seguir al jugador
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // Implementa el código para seguir al jugador aquí
        Transform player = GameObject.FindWithTag("Player").transform;
        float step = 2f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);
    }
}
