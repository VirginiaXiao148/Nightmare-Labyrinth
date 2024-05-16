using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AricController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public Camera mainCamera;
    public int maxHealth = 100;
    public Image healthBar;
    public Text healthText;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    private int currentHealth;

    //private MazeGeneratorOptimized1 mazeGenerator;
    private MazeGenerator mazeGenerator;
    private CharacterController controller;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 3f;
    private float jumpHeight = 0.5f;
    private float gravityValue = -9.81f;

    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentHealth = maxHealth;
        animator = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animator.SetInteger("Health", currentHealth);

        mazeGenerator = GameObject.FindObjectOfType<MazeGenerator>();

        if (mazeGenerator != null)
        {
            Vector2Int startPosition = mazeGenerator.GetStartCellPosition();
            transform.position = new Vector3(startPosition.x, 0f, startPosition.y);
        }

        controller = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        try
        {
            HandleMovement();
            HandleJumping();
            HandleAnimations();
            HandleAttack();
            UpdateHealth();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error in Update method: " + ex.Message);
        }
    }

    private void HandleMovement()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * vertical + right * horizontal;

        if (moveDirection != Vector3.zero)
        {
            controller.Move(moveDirection * playerSpeed * Time.deltaTime);
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump Button Pressed");
            if (groundedPlayer)
            {
                Debug.Log("Player is Grounded - Executing Jump");
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
            else
            {
                Debug.Log("Player is not Grounded - Jump not Executed");
            }
        }
    }

    private void HandleAnimations()
    {
        animator.SetBool("Forward", Input.GetKey(KeyCode.W));
        animator.SetBool("Backward", Input.GetKey(KeyCode.S));
        animator.SetBool("Right", Input.GetKey(KeyCode.D));
        animator.SetBool("Left", Input.GetKey(KeyCode.A));
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Attempt to attack");
            animator.SetTrigger("Attack");
            StartCoroutine(HandleDelayedAttack());
        }
    }

    private IEnumerator HandleDelayedAttack()
    {
        yield return new WaitForSeconds(1f);
        audioSource.Play();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
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

    private void UpdateHealth()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        healthText.text = currentHealth.ToString();
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.Log("The character has taken damage: " + damageAmount);
        currentHealth -= damageAmount;
        animator.SetBool("DamageTaken", true);
        if (currentHealth <= 0)
        {
            Die();
        }
        StartCoroutine(ResetTakeDamageAnimation());
    }

    private IEnumerator ResetTakeDamageAnimation()
    {
        yield return new WaitForSeconds(2f);
        animator.SetBool("DamageTaken", false);
    }

    private void Die()
    {
        animator.SetBool("Death", true);
        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }
}
