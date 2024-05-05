using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AricController1 : MonoBehaviour
{

    // Speed at which the player rotates
    public float rotationSpeed = 5f;

    // Reference to the main camera
    public Camera mainCamera;

    public int maxHealth = 100;
    public int attackDamage = 10;
    int currentHealth;

    private MazeGenerator mazeGenerator;
    // Character controller component
    private CharacterController controller;

    // Player movement variables
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 3f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    //bool Forward, Backward, Right, Left;

    // Animator component for character animation
    private Animator animator;

    // Called when the script is initialized
    private void Start()
    {
        // Set the current health
        currentHealth = maxHealth;

        // Get the Animator component attached to the GameObject
        animator = gameObject.GetComponent<Animator>();

        // We set the health of the character
        animator.SetInteger("Health", currentHealth);

        mazeGenerator = GameObject.FindObjectOfType<MazeGenerator>();

        if (mazeGenerator != null)
        {
            // Obtener la posici�n de inicio del laberinto
            Vector2Int startPosition = mazeGenerator.GetStartCellPosition();

            // Establecer la posici�n inicial del personaje
            transform.position = new Vector3(startPosition.x, 0f, startPosition.y);
        }

        // Get the CharacterController component attached to the GameObject
        controller = gameObject.GetComponent<CharacterController>();
    }

    // Called every frame
    private void Update()
    {
        // Check if the player is grounded
        groundedPlayer = controller.isGrounded;

        // Reset player's vertical velocity if grounded
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Obtener las entradas de movimiento relativas al jugador
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Convertir las entradas en un vector de movimiento basado en la orientación de la cámara
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0; // Asegurar que el movimiento no tenga componente vertical
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Vector de movimiento basado en la cámara
        Vector3 moveDirection = forward * vertical + right * horizontal;

        // Mover el personaje
        controller.Move(moveDirection * playerSpeed * Time.deltaTime);

        // Rotar al personaje para que mire en la dirección de movimiento
        if (moveDirection != Vector3.zero) {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Jumping mechanism
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        
        // Apply gravity to player's vertical velocity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Apply vertical movement to the player
        controller.Move(playerVelocity * Time.deltaTime);

        // The character will display the animations
        HandleAnimations();

        // Handle player attack
        HandleAttack();

        // Print the health of characters
        Debug.Log(this.getHealth());
    }

    string getHealth(){
        string value_health = "MainP: " + this.currentHealth;
        return value_health;
    }

    void HandleAnimations(){

        // Check keyboard input to control movement animations
        animator.SetBool("Forward", Input.GetKey(KeyCode.W));
        animator.SetBool("Backward", Input.GetKey(KeyCode.S));
        animator.SetBool("Right", Input.GetKey(KeyCode.D));
        animator.SetBool("Left", Input.GetKey(KeyCode.A));
        
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Attempt to attack");
            Debug.Log("Mouse button pressed");
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            animator.SetTrigger("Attack");
            if (Physics.Raycast(ray, out hit, 5f))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);
                SpiderController spiderController = hit.collider.GetComponent<SpiderController>();
                if (spiderController != null)
                {
                    spiderController.TakeDamage(attackDamage);
                    
                    Debug.Log("Enemy hit");
                }
            }
        }
    }

    public float animationTime = 5f;
    // Method to apply damage to the player
    public void TakeDamage(int damageAmount)
    {
        Debug.Log("El personaje ha sufrido daño" + damageAmount);
        // Reduce health by damage amount
        currentHealth -= damageAmount;
        // Update the health of the character
        animator.SetBool("DamageTaken",true);
        // Check if the player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
        StartCoroutine(ResetTakeDamageAnimation());
    }

    private IEnumerator ResetTakeDamageAnimation(){
        yield return new WaitForSeconds(animationTime);
        animator.SetBool("DamageTaken",false);
    }

    // Method to handle player death
    void Die()
    {
        animator.SetBool("Death",true);
        // Add code here to handle player death, such as restarting the level or displaying a game over screen
        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }
}