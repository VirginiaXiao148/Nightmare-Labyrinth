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
        if (!isStunned)
        {
            MoveTowardsPlayer();
            CheckForPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (isStunned)
        {
            return;
        }

        // Establecer el parámetro de animación para caminar
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
            if (Physics.Raycast(transform.position, raycastDir, out hit, obstacleAvoidanceDistance))
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
        //transform.position += transform.forward * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
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
        if (isStunned || player == null)
        {
            return;
        }

        GetComponent<Animation>().Stop();

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