using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demon : MonoBehaviour
{
    public float maxHealthDemon = 50;
    private float currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 5f;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    public float attackCooldown = 1.5f;
    private Transform player;
    private float lastAttackTime;

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing from this game object");
        }
        else
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        currentHealth = maxHealthDemon;
        healthBar.SetHealth(currentHealth, maxHealthDemon);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        animator.SetBool("Walking", true);

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
            animator.SetBool("Punching1", true);
            player.GetComponent<AricController>().TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        animator.SetBool("Stunned", true);
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth, maxHealthDemon);
        Debug.Log("Demon takes " + damage + " damage, health is now " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Demon died!");
        gameObject.SetActive(false);
    }
}
