using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tormented : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Encuentra la posición del jugador
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer < chaseRange)
        {
            // Mover hacia el jugador
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Moverse aleatoriamente
            MoveRandomly();
        }
    }

    void MoveRandomly()
    {
        // Implementa el movimiento aleatorio aquí
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        transform.position += randomDirection * moveSpeed * Time.deltaTime;
    }

    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            // Implementa el código para atacar al jugador
            player.GetComponent<AricController>().TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }
}
