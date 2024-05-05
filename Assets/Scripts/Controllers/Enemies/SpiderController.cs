using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour{

    public float moveSpeed = 3f; // Speed of enemy movement
    private Vector3 moveDirection; // Current movement direction of the enemy

    private float changeDirectionInterval = 2f; // Time interval to change movement direction
    private float timer; // Timer to control direction change

    public int maxHealthSpider = 30;

    public float attackRange = 1f;
    public int attackDamage = 5;
    public float attackCooldown = 2f; // Time between attacks
    private float lastAttackTime; // Time of the last attack

    public string id;

    public float fieldOfViewAngle = 90f; // Enemy's field of view angle (in degrees)
    public float maxSightDistance = 10f; // Maximum distance at which the enemy can detect the player

    private int currentHealth; // Current health of the enemy

    private Transform player; // Reference to the player's transform

    private Animation animation; // Reference to the animation component

    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        id = System.Guid.NewGuid().ToString();

        currentHealth = maxHealthSpider; // Initialize current health to maximum health
        animation = GetComponent<Animation>(); // Get reference to the Animation component attached to this GameObject

        player = GameObject.FindGameObjectWithTag("Player").transform; // Find and store a reference to the player's transform

        // Initially, assign a random movement direction to the enemy
        ChangeDirection();

        Debug.Log("Current health "+currentHealth);
    }

    void Update(){
        // Update the timer
        timer += Time.deltaTime;

        // Mover al enemigo en la dirección establecida a la velocidad configurada
        //transform.position += moveDirection * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position += moveDirection * moveSpeed * Time.deltaTime;
        rb.MovePosition(newPosition);

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

        Debug.Log(getLife());

    }

    string getLife(){
        string txt = "Araña " + id + " y su vida es " + currentHealth;
        return txt;
    }

    bool IsBlocked(Vector3 direction){
        RaycastHit hit;
        return Physics.Raycast(transform.position, direction, out hit, 7f) && hit.collider.CompareTag("Wall");
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

            if (!IsBlocked(randomDirection)){
                foundValidDirection = true;
            }
        }

        // Set the new direction of movement
        moveDirection = randomDirection;

        // Mover al enemigo en la dirección establecida a la velocidad configurada
        //transform.position += moveDirection * moveSpeed * Time.deltaTime;

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
        
        Vector3 newPosition = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }


    void AttackPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, maxSightDistance))
        {
            Debug.Log("Hit: " + hit.collider.name);  // Esto te dirá qué objeto está golpeando el raycast
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player in sight and hit by raycast");
                // Play the attack animation
                animation.Play("Attack");
                // Deal damage to the player (you should call a function in the player's script to handle damage logic)
                player.GetComponent<AricController1>().TakeDamage(attackDamage);
                // Record the time of the last attack
                lastAttackTime = Time.time;
            }
        }
    }

    private float knockbackForce = 5f; // Force of the knockback effect

    // Function to handle when the enemy takes damage
    public void TakeDamage(int damage)
    {
        // This stops all animations that are currently playing
        GetComponent<Animation>().Stop();
        // Decrease the enemy's health by the specified damage amount
        currentHealth -= damage;
        Debug.Log("Enemy takes " + damage + " damage, health is now " + currentHealth);
        // Knockback the enemy when taking damage
        // Calculate the knockback direction
        Vector3 knockbackDirection = (transform.position - player.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        // Check if the enemy's health has reached zero or below
        if (currentHealth <= 0)
        {
            Debug.Log("The enemy has no health left ");
            // If health is zero or below, call the Die function
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        // disable the gameobject
        gameObject.SetActive(false);
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
