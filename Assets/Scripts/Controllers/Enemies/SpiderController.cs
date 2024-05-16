using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderController : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    private Vector3 moveDirection;

    private bool isStunned = false;

    private float changeDirectionInterval = 2f;
    private float timer;

    public float maxHealthSpider = 30;
    private float currentHealth;
    public HealthBar healthBar;

    public float attackRange = 1.5f;
    public int attackDamage = 5;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    public string id;

    public float fieldOfViewAngle = 90f;
    public float detectionRadius = 10f;

    private Transform player;
    public LayerMask playerLayer;

    private Animation animation;
    private Rigidbody rb;

    private float knockbackForce = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        id = System.Guid.NewGuid().ToString();

        currentHealth = maxHealthSpider;
        healthBar.SetHealth(currentHealth, maxHealthSpider);

        animation = GetComponent<Animation>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        ChangeDirection();

        Debug.Log("Current health " + currentHealth);
    }

    void Update()
    {
        if (!isStunned)
        {
            timer += Time.deltaTime;
            if (timer >= changeDirectionInterval)
            {
                ChangeDirection();
                timer = 0f;
            }

            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);

            if (IsPlayerInSight())
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
                else if (distanceToPlayer > attackRange)
                {
                    FollowPlayer();
                }
            }
        }

        Debug.Log(getLife());
    }

    string getLife()
    {
        return "Araña " + id + " y su vida es " + currentHealth;
    }

    bool IsBlocked(Vector3 direction)
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, direction, out hit, 3f) && hit.collider.CompareTag("Wall");
    }

    void ChangeDirection()
    {
        if (!animation.IsPlaying("Walk"))
        {
            PlayWalkAnimation();
        }

        bool foundValidDirection = false;
        Vector3 randomDirection = Vector3.zero;

        while (!foundValidDirection)
        {
            float randomX = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);
            randomDirection = new Vector3(randomX, 0f, randomZ).normalized;

            if (!IsBlocked(randomDirection))
            {
                foundValidDirection = true;
            }
        }

        moveDirection = randomDirection;
        transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
    }

    bool IsPlayerInSight()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player seen!");
                return true;
            }
        }
        return false;
    }

    void FollowPlayer()
    {
        if (!animation.IsPlaying("Run"))
        {
            PlayRunAnimation();
        }

        Vector3 newPosition = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

    void AttackPlayer()
    {
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