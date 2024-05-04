using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float playerSpeed = 0.3f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    // Animator component for character animation
    private Animator animator;

    // Called when the script is initialized
    private void Start()
    {

        // Get the Animator component attached to the GameObject
        animator = gameObject.GetComponent<Animator>();

        currentHealth = maxHealth;
        animator.SetInteger("Health",currentHealth);

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

        // Get input for player movement
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Move the player based on input
        controller.Move(move * Time.deltaTime * playerSpeed);

        /**
         * // Rotate the player to face the movement direction
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }**/

        // Jumping mechanism
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        
        // Apply gravity to player's vertical velocity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Apply vertical movement to the player
        controller.Move(playerVelocity * Time.deltaTime);

        // Rotate the player based on the rotation of the camera
        RotatePlayer();

        // The character will display the animations
        HandleAnimations();

        // Handle player attack
        HandleAttack();
    }

    void RotatePlayer()
    {
        // Get the horizontal input for player rotation
        float rotationInput = Input.GetAxis("Horizontal");

        // Calculate the rotation angle based on input and rotation speed
        float rotationAngle = rotationInput * rotationSpeed * Time.deltaTime;

        // Rotate the player around the y-axis based on the camera's rotation
        // Get the camera's forward vector projected onto the XZ plane
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0f; // Ignore the vertical component

        // Rotate the player towards the direction the camera is facing
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationAngle);
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
            // Raycast desde la posici�n del rat�n para detectar objetos en el mundo
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Comprueba si el objeto clicado es un enemigo u otro objetivo v�lido para el ataque
                if (hit.collider.CompareTag("Spider"))
                {
                    // Activa la animaci�n de ataque del personaje
                    animator.SetTrigger("Attack");

                    // Aplica efectos de ataque al enemigo, como da�o
                    SpiderController spiderController = hit.collider.GetComponent<SpiderController>();
                    if (spiderController != null)
                    {
                        spiderController.TakeDamage(attackDamage);
                    }
                }

                /**
                 * 
                 * else if (hit.collider.CompareTag("Phantom"))
                {
                    // Activa la animaci�n de ataque del personaje
                    animator.SetTrigger("Attack");

                    // Aplica efectos de ataque al enemigo, como da�o
                    EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(attackDamage);
                    }
                } else if (hit.collider.CompareTag("Illusion"))
                {
                    // Activa la animaci�n de ataque del personaje
                    animator.SetTrigger("Attack");

                    // Aplica efectos de ataque al enemigo, como da�o
                    EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(attackDamage);
                    }
                }
                 * **/
            }
        }
    }

    // Method to apply damage to the player
    public void TakeDamage(int damageAmount)
    {
        // Reduce health by damage amount
        currentHealth -= damageAmount;

        animator.SetInteger("Health",currentHealth);

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            // Destroy the enemy GameObject
            Destroy(gameObject);

            // End the game
            SceneManager.LoadScene("EndGameScene");
        }
    }

}