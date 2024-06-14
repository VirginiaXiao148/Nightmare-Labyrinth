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

    public float patrolRange = 10f; // Rango máximo en el que el demonio patrullará
    private Vector3 patrolPoint;   // Punto de patrullaje actual
    private bool isPatrolling = false; // Indica si el demonio está patrullando


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
    }

    /* private void Update()
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
    } */

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (!isStunned)
        {
            // Calcula la dirección hacia el jugador
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            // Si el jugador está dentro del rango de detección y está en la dirección frontal, ataca
            if (Vector3.Distance(transform.position, player.position) <= detectionRadius && angleToPlayer <= 60f)
            {
                MoveTowardsPlayer();

                // Si el jugador está dentro del rango de ataque y ha pasado el cooldown
                if (Vector3.Distance(transform.position, player.position) <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else
            {
                // Llama al método de patrullaje si el jugador no está en rango
                Patrolling();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        if (isStunned)
        {
            return;
        }

        animator.SetBool("Walking", true);

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void AttackPlayer()
    {
        if (isStunned || player == null)
        {
            return;
        }

        Debug.Log("Player detected! Attacking...");
        animator.SetBool("Walking", false);
        animator.SetBool("Punching1", true);
        player.GetComponent<AricController>().TakeDamage(attackDamage);
        lastAttackTime = Time.time;
    }

    public void TakeDamage(float damage)
    {
        animator.SetBool("Stunned", true);
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth, maxHealth);
        Debug.Log("Demon takes " + damage + " damage, health is now " + currentHealth);

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
        Debug.Log("Demon died!");
        animator.SetBool("Dead", true);
        gameObject.SetActive(false);
    }

    private void Patrolling()
    {
        // Si no está patrullando, elige un nuevo punto de patrullaje aleatorio
        if (!isPatrolling)
        {
            isPatrolling = true;
            StartCoroutine(ChoosePatrolPoint());
        }

        // Mueve hacia el punto de patrullaje actual si no hay obstáculos en el camino
        animator.SetBool("Walking", true);

        Vector3 direction = (patrolPoint - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Calcula la posición objetivo hacia la cual moverse
        Vector3 targetPosition = transform.position + transform.forward * moveSpeed * Time.deltaTime;

        // Verifica si hay un obstáculo en la posición objetivo
        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, 0.5f); // Ajusta el radio según el tamaño del demonio

        bool obstacleDetected = false;
        foreach (Collider collider in hitColliders)
        {
            // Verifica si hay colisión con alguna pared u obstáculo
            if (collider.CompareTag("Wall"))
            {
                obstacleDetected = true;
                break;
            }
        }

        // Si no hay obstáculos, mueve al demonio
        if (!obstacleDetected)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            transform.position = targetPosition;
        }
        else
        {
            // Hay un obstáculo, elige un nuevo punto de patrullaje
            isPatrolling = false;
        }

        // Si el demonio llega al punto de patrullaje actual, elige un nuevo punto
        if (Vector3.Distance(transform.position, patrolPoint) <= 0.1f)
        {
            isPatrolling = false;
        }
    }

    private IEnumerator ChoosePatrolPoint()
    {
        // Elige un nuevo punto de patrullaje aleatorio dentro del rango especificado
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        patrolPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Espera un tiempo aleatorio antes de elegir el siguiente punto de patrullaje
        float waitTime = Random.Range(3f, 8f);
        yield return new WaitForSeconds(waitTime);

        isPatrolling = false;
    }

}
