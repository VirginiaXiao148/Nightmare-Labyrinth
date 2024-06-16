using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiderController : MonoBehaviour
{
    public float moveSpeed = 0.3f;
    public float rotationSpeed = 5f;
    public float detectionRadius = 10f;
    public float fieldOfViewAngle = 90f;
    public float maxSightDistance = 10f;

    public float attackRange = 1.5f;
    public int attackDamage = 5;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    public float maxHealthSpider = 30f;
    private float currentHealth;
    public HealthBar healthBar;

    private Transform player;

    private Animation animation;
    private Rigidbody rb;

    private bool isStunned = false;
    public float knockbackForce = 5f;

    private float changeDirectionInterval = 2f;
    private float timer;
    private Vector3 moveDirection;

    public float obstacleAvoidanceDistance = 2.0f;
    public LayerMask obstacleLayerMask;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animation = GetComponent<Animation>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        currentHealth = maxHealthSpider;
        healthBar.SetHealth(currentHealth, maxHealthSpider);

        moveDirection = Vector3.forward;
    }

    private void Update()
    {
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
                if (timer >= changeDirectionInterval)
                {
                    ChangeDirection();
                    timer = 0f;
                }
            }
        }
    }

    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < fieldOfViewAngle / 2f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, maxSightDistance))
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
        if (!animation.IsPlaying("Walk"))
        {
            PlayWalkAnimation();
        }

        Vector3 randomDirection = Vector3.zero;
        bool foundValidDirection = false;
        Vector3[] raycastDirections = new Vector3[]
        {
            transform.forward,
            transform.forward + transform.right,
            transform.forward - transform.right
        };

        // Debugging ray visualization
        foreach (Vector3 raycastDir in raycastDirections)
        {
            Debug.DrawRay(transform.position, raycastDir * obstacleAvoidanceDistance, Color.red);
        }

        foreach (Vector3 raycastDir in raycastDirections)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, raycastDir, out hit, obstacleAvoidanceDistance, obstacleLayerMask))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    // Calculate new direction to avoid the obstacle
                    Vector3 newDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                    if (newDirection != Vector3.zero)
                    {
                        randomDirection = newDirection;
                        foundValidDirection = true;
                        break; // Exit the loop if a valid direction is found
                    }
                }
            }
        }

        if (!foundValidDirection)
        {
            // If no valid direction was found via raycasting, choose a random direction
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        }

        moveDirection = randomDirection;
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
    }

    private void MoveInDirection()
    {
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    private void MoveTowardsPlayer()
    {
        if (!animation.IsPlaying("Walk"))
        {
            PlayWalkAnimation();
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
                    // Calcular nueva dirección para evitar el obstáculo
                    Vector3 newDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                    if (newDirection != Vector3.zero)
                    {
                        direction = newDirection;
                        break; // Salir del bucle si se encontró una dirección válida
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
        animation.Play("Attack");
        player.GetComponent<AricController>().TakeDamage(attackDamage);
        lastAttackTime = Time.time;
    }

    public void TakeDamage(float damage)
    {
        animation.Stop();
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth, maxHealthSpider);

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
    }

    private void Die()
    {
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

    private bool IsBlocked(Vector3 direction)
    {
        return Physics.Raycast(transform.position, direction, obstacleAvoidanceDistance, obstacleLayerMask);
    }
}