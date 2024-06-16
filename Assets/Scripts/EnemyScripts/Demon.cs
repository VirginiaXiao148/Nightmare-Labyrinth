using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonController : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 0.3f;
    public float rotationSpeed = 5f;
    public float detectionRadius = 10f;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    public float attackCooldown = 3f;
    private float lastAttackTime;

    public float obstacleAvoidanceDistance = 2.0f;
    public LayerMask obstacleLayerMask;

    private Transform player;
    private Animator animator;
    private Rigidbody rb;

    private bool isStunned = false;
    public float knockbackForce = 5f;

    private float timer;
    public float changeDirectionInterval = 2f;
    private Vector3 moveDirection;

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
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
        lastAttackTime = -attackCooldown;

        moveDirection = Vector3.forward;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (!isStunned)
        {
            timer += Time.deltaTime;

            if (timer >= changeDirectionInterval)
            {
                ChangeDirection();
                timer = 0f;
            }

            if (IsPlayerInSight())
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
                else if (distanceToPlayer > attackRange)
                {
                    MoveTowardsPlayer();
                }
            }
            else
            {
                MoveInDirection();
            }
        }
    }

    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < 60f) // Assuming a field of view angle of 120 degrees divided by 2
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ChangeDirection()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            animator.SetBool("Walking", true);
        }
        
        Vector3 randomDirection = Vector3.zero;
        Vector3[] raycastDirections = new Vector3[]
        {
            transform.forward,
            transform.forward + transform.right,
            transform.forward - transform.right
        };

        foreach (Vector3 raycastDir in raycastDirections)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, raycastDir, out hit, obstacleAvoidanceDistance, obstacleLayerMask))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    // Calcular nueva dirección para evitar el obstáculo
                    Vector3 newDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                    if (newDirection != Vector3.zero)
                    {
                        randomDirection = newDirection;
                        break; // Salir del bucle si se encontró una dirección válida
                    }
                }
            }
        }

        moveDirection = randomDirection;
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
    }

    private void MoveInDirection()
    {
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
    }

    private void MoveTowardsPlayer()
    {
        if (isStunned)
        {
            return;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            animator.SetBool("Walking", true);
        }

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
            if (Physics.Raycast(transform.position, raycastDir, out hit, obstacleAvoidanceDistance, obstacleLayerMask))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    // Calculate new direction to avoid obstacle
                    Vector3 newDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                    if (newDirection != Vector3.zero)
                    {
                        direction = newDirection;
                        break; // Exit loop if valid direction found
                    }
                }
            }
        }

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    private void AttackPlayer()
    {
        if (isStunned || player == null)
        {
            return;
        }

        animator.SetBool("Walking", false);
        animator.SetBool("Punching1", true);

        Vector3 directionToPlayer = player.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
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
        animator.SetBool("Stunned", false);
    }

    private void Die()
    {
        animator.SetBool("Dead", true);
        Destroy(gameObject, 2f);
    }

    private bool IsBlocked(Vector3 direction)
    {
        return Physics.Raycast(transform.position, direction, obstacleAvoidanceDistance, obstacleLayerMask);
    }
}