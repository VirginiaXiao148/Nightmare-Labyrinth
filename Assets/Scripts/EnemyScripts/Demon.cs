using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DemonController : MonoBehaviour
{
    public float maxHealth = 50;
    private float currentHealth;
    public HealthBar healthBar;

    public float detectionRadius = 10f;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    public float attackCooldown = 3f;
    private float lastAttackTime;

    private Transform player;
    private Animator animator;
    private NavMeshAgent agent;

    private bool isStunned = false;
    public float knockbackForce = 5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
            return;
        }

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component missing from this game object");
            return;
        }

        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
        lastAttackTime = -attackCooldown;

        // Ajustar la posición del agente a la NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogError("Demon not placed on a valid NavMesh position.");
            return;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        MoveTowardsPlayer();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (isStunned || !agent.isOnNavMesh)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            return;
        }

        // Establecer el parámetro de animación para caminar
        animator.SetBool("Walking", true);

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        if (isStunned || player == null)
        {
            return;
        }

        Debug.Log("Player detected! Attacking...");
        Vector3 directionToPlayer = player.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius))
        {
            Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player in sight and hit by raycast");
                animator.SetBool("Walking", false);
                animator.SetBool("Punching1", true);
                player.GetComponent<AricController>().TakeDamage(attackDamage);
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        animator.SetBool("Stunned", true);
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth, maxHealth);
        Debug.Log("Demon takes " + damage + " damage, health is now " + currentHealth);

        Vector3 knockbackDirection = (transform.position - player.position).normalized;
        if (agent.isOnNavMesh)
        {
            agent.velocity = knockbackDirection * knockbackForce;
        }

        if (!isStunned)
        {
            StartCoroutine(StunEnemy(2.0f));
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator StunEnemy(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        animator.SetBool("Stunned", false);
    }

    private void Die()
    {
        Debug.Log("Demon died!");
        animator.SetBool("Dead", true);
        gameObject.SetActive(false);
    }
}
