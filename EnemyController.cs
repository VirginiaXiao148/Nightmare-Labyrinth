using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 1f;
    public float attackDamage = 5;
    public int maxHealth = 50;

    private int currentHealth;

    private Transform player;

    // El tamaño de tu laberinto
    private int mazeWidth = 50;
    private int mazeHeight = 50;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;

        // Establecer la posición inicial del jugador
        transform.position = GetRandomStartPosition();
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                // Atacar al jugador
                AttackPlayer();
            }
            else
            {
                // Seguir al jugador
                FollowPlayer();
            }
        }
    }

    /* void FollowPlayer()
    {
        transform.LookAt(player.position);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    } */

    void FollowPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // Implementar la lógica de ataque al jugador
        Debug.Log("Enemy attacking player!");
    }

    /* public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Realizar alguna acción cuando el enemigo recibe daño
            Debug.Log("Enemy took damage!");
        }
    } */

    private bool isInvulnerable = false;
    public float invulnerabilityTime = 1.0f;

    public void TakeDamage(int damage)
    {
        if (!isInvulnerable)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                // Realizar alguna acción cuando el enemigo recibe daño
                Debug.Log("Enemy took damage!");

                // Activar invulnerabilidad temporal
                isInvulnerable = true;
                Invoke("ResetInvulnerability", invulnerabilityTime);
            }
        }
    }

    void ResetInvulnerability()
    {
        isInvulnerable = false;
    }

    void Die()
    {
        // Implementar la lógica cuando el enemigo muere
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    Vector3 GetRandomStartPosition()
    {
        // Obtener una posición aleatoria dentro del laberinto
        int randomX = Random.Range(1, mazeWidth - 1);  // Ajusta según el tamaño de tu laberinto
        int randomZ = Random.Range(1, mazeHeight - 1);

        // Convertir las coordenadas a posiciones en Unity
        Vector3 startPosition = new Vector3(randomX, 0f, randomZ);

        return startPosition;
    }
}