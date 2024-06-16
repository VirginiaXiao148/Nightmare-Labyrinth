using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SpiderController : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float rotationSpeed = 5f;
    public float detectionRadius = 10f;

    public float attackRange = 1.5f;
    public int attackDamage = 5;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    public float maxHealthSpider = 30;
    private float currentHealth;
    public HealthBar healthBar;

    private Transform player;

    private Animation animation;
    private Rigidbody rb;
    private NavMeshAgent navMeshAgent;

    private bool isStunned = false;
    public float knockbackForce = 5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animation = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        currentHealth = maxHealthSpider;
        healthBar.SetHealth(currentHealth, maxHealthSpider);

        navMeshAgent.speed = moveSpeed;
        navMeshAgent.stoppingDistance = attackRange;

        // Ajustar la posición del agente a la NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            navMeshAgent.Warp(hit.position);
        }
        else
        {
            Debug.LogError("Spider not placed on a valid NavMesh position.");
            return;
        }
    }

    private void Update()
    {
        if (player == null || !navMeshAgent.isOnNavMesh)
        {
            return;
        }

        if (!isStunned)
        {
            MoveTowardsPlayer();
            CheckForPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        if (!animation.IsPlaying("Walk"))
        {
            PlayWalkAnimation();
        }

        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }

    private void CheckForPlayer()
    {
        if (player == null)
        {
            SceneManager.LoadScene("EndGame");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("Player detected! Attacking...");
        Vector3 directionToPlayer = player.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius))
        {
            Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player in sight and hit by raycast");
                animation.Play("Attack");
                player.GetComponent<AricController>().TakeDamage(attackDamage);
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        animation.Stop();
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth, maxHealthSpider);
        Debug.Log("Enemy takes " + damage + " damage, health is now " + currentHealth);

        Vector3 knockbackDirection = (transform.position - player.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

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
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }
        yield return new WaitForSeconds(duration);
        isStunned = false;
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = false;
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        animation.Play("Death");
        StartCoroutine(DisableAfterAnimation(animation["Death"].length));
    }

    private IEnumerator DisableAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    public void PlayWalkAnimation()
    {
        animation.Play("Walk");
    }

    public void PlayRunAnimation()
    {
        animation.Play("Run");
    }

    public void PlayIdleAnimation()
    {
        animation.Play("Idle");
    }
}
