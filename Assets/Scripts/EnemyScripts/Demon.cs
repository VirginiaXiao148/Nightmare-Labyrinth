using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemonController : MonoBehaviour
{
    public float maxHealth = 50;
    private float currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 0.3f;
    public float rotationSpeed = 5f;
    public float detectionRadius = 10f;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    public float attackCooldown = 3f;
    private float lastAttackTime;

    private Transform player;
    private Animator animator;
    private Rigidbody rb;

    private bool isStunned = false;
    public float knockbackForce = 5f;

    public float obstacleAvoidanceDistance = 2.0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
            return;
        }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing from this game object");
            return;
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; // Cambio aquí
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Suaviza el movimiento

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Constrain rotations

        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
        lastAttackTime = -attackCooldown;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (!isStunned)
        {
            MoveTowardsPlayer();

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        animator.SetBool("Walking", true);

        Vector3 direction = (player.position - transform.position).normalized;
        Vector3[] raycastDirections = new Vector3[]
        {
            transform.forward,
            transform.forward + transform.right,
            transform.forward - transform.right
        };

        foreach (Vector3 raycastDir in raycastDirections)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, raycastDir, out hit, obstacleAvoidanceDistance))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    Vector3 newDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                    if (newDirection != Vector3.zero)
                    {
                        direction = newDirection;
                        break;
                    }
                }
            }
        }

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
    }

    private void AttackPlayer()
    {
        Debug.Log("Player detected! Attacking...");
        Vector3 directionToPlayer = player.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                animator.SetBool("Walking", false);
                animator.SetTrigger("Punching1");
                player.GetComponent<AricController>().TakeDamage(attackDamage);
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("Stunned");
        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

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
        yield return new WaitForSeconds(duration);
        isStunned = false;
        animator.ResetTrigger("Stunned");
    }

    private void Die()
    {
        Debug.Log("Demon died!");
        animator.SetTrigger("Dead");
        gameObject.SetActive(false);
    }
}
