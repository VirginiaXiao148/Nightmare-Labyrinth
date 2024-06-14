using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpiderController : MonoBehaviour
{
    public float moveSpeed = 0.3f;
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

    public float patrolRange = 10f;
    private Vector3 patrolPoint; 
    private bool isPatrolling = false;

    private bool isStunned = false;
    public float knockbackForce = 5f;

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
    }

    private void Update()
    {
        /* if (!isStunned)
        {
            MoveTowardsPlayer();
            CheckForPlayer();
        } */
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

    /* private void MoveTowardsPlayer()
    {
        if (player == null) return;

        if (!animation.IsPlaying("Walk"))
        {
            PlayWalkAnimation();
        }

        Vector3 direction = (player.position - transform.position).normalized;

        // Detectar obstáculos
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceDistance))
        {
            Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.Log("Wall detected by raycast");

                // Calcular nueva dirección para evitar el obstáculo
                Vector3 newDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                // Asegurar que la dirección no sea cero
                if (newDirection != Vector3.zero)
                {
                    direction = newDirection;
                }

                // Aplicar una pequeña fuerza para alejarse de la pared
                rb.AddForce(-hit.normal * knockbackForce, ForceMode.Impulse);
            }
        }

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    } */

    private void MoveTowardsPlayer()
    {
        if (isStunned)
        {
            return;
        }

        if (!animation.IsPlaying("Walk"))
        {
            PlayWalkAnimation();
        }

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
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
        GetComponent<Animation>().Stop();
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
        yield return new WaitForSeconds(duration);
        isStunned = false;
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

    private void Patrolling()
    {
        // Si no está patrullando, elige un nuevo punto de patrullaje aleatorio
        if (!isPatrolling)
        {
            isPatrolling = true;
            StartCoroutine(ChoosePatrolPoint());
        }

        // Mueve hacia el punto de patrullaje actual si no hay obstáculos en el camino
        if (!animation.IsPlaying("Walk"))
        {
            PlayWalkAnimation();
        }

        Vector3 direction = (patrolPoint - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Calcula la posición objetivo hacia la cual moverse
        Vector3 targetPosition = transform.position + transform.forward * moveSpeed * Time.deltaTime;

        // Verifica si hay un obstáculo en la posición objetivo usando Raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, obstacleAvoidanceDistance, obstacleLayerMask))
        {
            Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Wall"))
            {
                Debug.Log("Wall detected by raycast");

                // Calcular nueva dirección para evitar el obstáculo
                Vector3 newDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                // Asegurar que la dirección no sea cero
                if (newDirection != Vector3.zero)
                {
                    direction = newDirection;
                    lookRotation = Quaternion.LookRotation(direction);
                }
            }
        }

        // Mueve al demonio hacia la dirección ajustada
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

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