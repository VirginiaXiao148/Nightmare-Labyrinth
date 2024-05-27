using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    public float attackCooldown = 1.5f;
    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Encuentra la posición del jugador
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        // Seguir al jugador implacablemente
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < attackRange)
        {
            AttackPlayer();
        }
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
