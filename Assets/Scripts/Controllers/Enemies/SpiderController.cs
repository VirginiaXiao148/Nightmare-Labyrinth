using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour{

    public float moveSpeed = 1f; // Speed of enemy movement
    private Vector3 moveDirection; // Current movement direction of the enemy

    private float changeDirectionInterval = 2f; // Time interval to change movement direction
    private float timer; // Timer to control direction change

    public int maxHealth = 30;

    public float attackRange = 1f;
    public int attackDamage = 5;
    public float attackCooldown = 2f; // Time between attacks
    private float lastAttackTime; // Time of the last attack

    public float fieldOfViewAngle = 90f; // Enemy's field of view angle (in degrees)
    public float maxSightDistance = 10f; // Maximum distance at which the enemy can detect the player

    private int currentHealth; // Current health of the enemy

    private Transform player; // Reference to the player's transform

    private Animation animation; // Reference to the animation component


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to maximum health
        animation = GetComponent<Animation>(); // Get reference to the Animation component attached to this GameObject

        player = GameObject.FindGameObjectWithTag("Player").transform; // Find and store a reference to the player's transform
        currentHealth = maxHealth; // Reset current health to maximum health

        // Initially, assign a random movement direction to the enemy
        ChangeDirection();
    }

    void Update(){
        // Update the timer
        timer += Time.deltaTime;

        // If the timer reaches the direction change interval, change the enemy's direction
        if (timer >= changeDirectionInterval)
        {
            ChangeDirection();
            // Reset the timer
            timer = 0f;
        }

        //check if the is in sight
        if (IsPlayerInSight()){
            // calculate the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(transform.position,player.position);

            if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer(); // Attack the player
            }
            else if (distanceToPlayer > attackRange)
            {
                FollowPlayer(); // Follow the player
            }
        }

    }

    bool IsBlocked(Vector3 direction){
        RaycastHit hit;
        return Physics.Raycast(transform.position, randomDirection, out hit, maxSightDistance);
    }
    
    // Change the enemy's movement direction to a random direction
    void ChangeDirection()
    {
        // Check if the "Walk" animation is not playing
        if (!animation.IsPlaying("Walk"))
        {
            // If not playing, play the "Walk" animation
            PlayWalkAnimation();
        }

        bool foundValidDirection = false;
        Vector3 randomDirection = Vector3.zero;

        // Keep searching for a valid direction until one is found
        while (!foundValidDirection)
        {
            // Generate a new random direction in the XY plane (horizontal)
            float randomX = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);
            randomDirection = new Vector3(randomX, 0f, randomZ).normalized;

            // Check if there's a wall in the new direction
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, randomDirection, out hit, maxSightDistance) || !hit.collider.CompareTag("Wall"))
            {
                // If no wall is hit or the hit object is not a wall, the direction is valid
                foundValidDirection = true;
            }
        }

        // Set the new direction of movement
        moveDirection = randomDirection;

        // Rotate the enemy to face the new direction
        transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
    }



    bool IsPlayerInSight()
    {
        // Direction from the spider to the player
        Vector3 directionToPlayer = player.position - transform.position;

        // Angle between the spider's forward direction and the direction to the player
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check if the player is within the spider's field of view angle and within sight range
        if (angleToPlayer < fieldOfViewAngle / 2f)
        {
            // Cast a ray from the spider towards the player to check for obstructions
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, maxSightDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // The player is within the field of view and there are no obstructions, so the spider can see the player
                    return true;
                }
            }
        }
        // The player is not within the field of view or is obstructed
        return false;
    }


    void FollowPlayer()
    {
        if (!animation.IsPlaying("Run"))
        {
            // If "Run" animation is not playing, play it
            PlayRunAnimation();
        }

        // Move the spider towards the player's position at a constant speed
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }


    void AttackPlayer()
    {
        // Play the attack animation
        animation.Play("Attack");

        // Deal damage to the player (you should call a function in the player's script to handle damage logic)
        player.GetComponent<AricController1>().TakeDamage(attackDamage);

        // Record the time of the last attack
        lastAttackTime = Time.time;
    }


    // Variable to track if the enemy is currently invulnerable
    private bool isInvulnerable = false;

    // Time duration for the enemy's invulnerability
    public float invulnerabilityTime = 5f;

    // Function to handle when the enemy takes damage
    public void TakeDamage(int damage)
    {
        // Check if the enemy is not currently invulnerable
        if (!isInvulnerable)
        {
            // Decrease the enemy's health by the specified damage amount
            currentHealth -= damage;

            // Check if the enemy's health has reached zero or below
            if (currentHealth <= 0)
            {
                // If health is zero or below, call the Die function
                Die();
            }
            else
            {
                Debug.Log("Enemy took damage!");

                // Activate temporary invulnerability
                isInvulnerable = true;

                // Invoke the ResetInvulnerability function after a specified duration
                Invoke("ResetInvulnerability", invulnerabilityTime);
            }
        }
    }

    // Disable invulnerability
    void ResetInvulnerability()
    {
        isInvulnerable = false;
    }

    void Die()
    {
        // Implement the logic when the enemy dies
        Debug.Log("Enemy died!");

        // If health reaches zero, play the death animation
        animation.Play("Death");

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }

    // Method to play the walk animation
    public void PlayWalkAnimation()
    {
        animation.Play("Walk");
    }

    // Method to play the run animation
    public void PlayRunAnimation()
    {
        animation.Play("Run");
    }

    // Method to play the idle animation
    public void PlayIdleAnimation()
    {
        animation.Play("Idle");
    }
}
