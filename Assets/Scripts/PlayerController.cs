using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float sprintMultiplier = 1.5f;

    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private float crawlSpeedMultiplier = 0.5f;

    [SerializeField]
    private float gravity = -9.81f;

    private CharacterController characterController;
    private Animator animator;

    private Vector3 velocity;
    private bool isGrounded = true;
    private bool isCrawling = false;
    private bool isSprinting = false;

    private Vector3 movementInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
    }

    private void Update()
    {
        // Check if grounded
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep grounded
        }

        // Handle input for movement
        HandleInput();

        // Handle sprint and crawl
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrawling;

        // Apply movement and animations
        HandleMovement();
        HandleRotation();
        UpdateAnimations();

        // Handle jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            HandleJump();
        }

        // Apply gravity
        ApplyGravity();
    }

    private void HandleInput()
    {
        // Reset movement input
        movementInput = Vector3.zero;

        // Check keys for movement directions
        if (Input.GetKey(KeyCode.W)) movementInput += Vector3.forward; // Move forward
        if (Input.GetKey(KeyCode.S)) movementInput += Vector3.back;    // Move backward
        if (Input.GetKey(KeyCode.A)) movementInput += Vector3.left;   // Move left
        if (Input.GetKey(KeyCode.D)) movementInput += Vector3.right;  // Move right

        // Normalize movement input for consistent speed
        movementInput = movementInput.normalized;
    }

    private void HandleMovement()
    {
        // Adjust speed based on sprint or crawl
        float currentSpeed = isCrawling ? speed * crawlSpeedMultiplier : (isSprinting ? speed * sprintMultiplier : speed);

        // Apply movement to the character controller
        Vector3 movement = movementInput * currentSpeed * Time.deltaTime;
        characterController.Move(movement);
    }

    private void HandleRotation()
    {
        // Rotate the player to face movement direction
        if (movementInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void UpdateAnimations()
    {
        bool isMoving = movementInput.magnitude > 0;

        // Update animations based on movement states
        animator.SetBool("walk", isMoving && !isSprinting && !isCrawling);
        animator.SetBool("sprint", isMoving && isSprinting);
        animator.SetBool("crawl", isMoving && isCrawling);
        animator.SetBool("isGround", isGrounded);
    }

    private void HandleJump()
    {
        // Jump velocity using v = sqrt(2 * height * -gravity)
        velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
        animator.SetTrigger("jump");
    }

    private void ApplyGravity()
    {
        // Apply gravity over time
        velocity.y += gravity * Time.deltaTime;

        // Apply gravity to the character controller
        characterController.Move(velocity * Time.deltaTime);
    }
}
