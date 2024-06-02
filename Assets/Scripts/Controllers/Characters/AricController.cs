using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public float mentalHealth = 100;
    private float currentMentalHealth;

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
        currentMentalHealth = mentalHealth;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animator.SetInteger("Health", currentHealth);

        mazeGenerator = FindObjectOfType<MazeGenerator>();

        if (mazeGenerator != null)
        {
            Vector2Int startPosition = mazeGenerator.GetStartCellPosition();
            transform.position = new Vector3(startPosition.x, 0f, startPosition.y);
        }

        controller = GetComponent<CharacterController>();
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

        Vector3 moveDirection = CalculateMoveDirection(horizontal, vertical);

        if (moveDirection != Vector3.zero)
        {
            controller.Move(moveDirection * playerSpeed * Time.deltaTime);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
    }

    private Vector3 CalculateMoveDirection(float horizontal, float vertical)
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return forward * vertical + right * horizontal;
    }

    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
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
            animator.SetTrigger("Attack");
            StartCoroutine(HandleDelayedAttack());
        }
    }

    private IEnumerator HandleDelayedAttack()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.Play();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {
            SpiderController spiderController = hit.collider.GetComponent<SpiderController>();
            if (spiderController != null)
            {
                spiderController.TakeDamage(attackDamage);
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
        yield return new WaitForSeconds(1f);
        animator.SetBool("DamageTaken", false);
    }

    public void TakeMentalDamage(float mentalDamage)
    {
        currentMentalHealth -= mentalDamage;
        animator.SetBool("DamageTaken", true);
        if (currentMentalHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetBool("Death", true);
        gameObject.SetActive(false);
        StartCoroutine(LoadEndSceneAfterDelay("EndScene", 2.0f));
    }

    private IEnumerator LoadEndSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
