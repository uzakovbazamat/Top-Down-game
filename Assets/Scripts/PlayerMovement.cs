using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField, Range(0f, 50f)] private float walkSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;

    [Header("Gravity")]
    [SerializeField] private float normalGravity = 1f;
    [SerializeField] private float fallGravity = 2f;
    [SerializeField] private float jumpGravity = 0.75f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Facing")]
    [SerializeField] private float facingDirection = 1f;

    // Input state
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpReleased;

    //Runtime state 
    private bool isGrounded;


    private void Reset()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!playerInput) playerInput = GetComponent<PlayerInput>();
        if (!groundCheck)
        {
            var found = transform.Find("GroundCheck");
            if (found != null)
            {
                groundCheck = found;
            }
        }
    }

    private void Start()
    {
        rb.gravityScale = normalGravity;
    }

    private void Update()
    {
        UpdateFacing();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        UpdateGroundedState();
        ApplyVariableGravity();
        ApplyMovement();
        ApplyJump();
    }



    // PLAYER'S WALKING MECHANICS
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void ApplyMovement()
    {
        float targetSpeed = moveInput.x * walkSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    // PLAYER'S JUMPING MECHANICS
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
            jumpReleased = false;
        }
        else
        {
            jumpReleased = true;
        }
    }

    private void ApplyJump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            jumpReleased = false;
        }

        if (!jumpReleased)
        {
            return;
        }

        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
        jumpReleased = false;
    }

    // GRAVITY
    private void UpdateGroundedState()
    {
        if (!groundCheck)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ApplyVariableGravity()
    {
        float verticalSpeed = rb.linearVelocity.y;

        if (verticalSpeed < -0.1f)
        {
            rb.gravityScale = fallGravity;
        }
        else if (verticalSpeed > 0.1f)
        {
            rb.gravityScale = jumpGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    // FACE FLIPPING
    private void UpdateFacing()
    {
        if (moveInput.x > 0.1f)
        {
            facingDirection = 1;
        }
        else if (moveInput.x < -0.1f)
        {
            facingDirection = -1;
        }

        transform.localScale = new Vector3(facingDirection, 1f, 1f);
    }

    // GIZMOS
    private void OnDrawGizmosSelected()
    {
        if (!isGrounded) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    // ANIMATIONS
    private void UpdateAnimations()
    {
        if (!animator)
        {
            return;
        }

        float verticalSpeed = rb.linearVelocity.y;
        float horizontalSpeed = Mathf.Abs(moveInput.x);

        animator.SetBool("isJumping", verticalSpeed > 0.1f);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", verticalSpeed);

        bool isIdle = horizontalSpeed < 0.1 && isGrounded;
        bool isWalking = horizontalSpeed > 0.1 && isGrounded;

        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isWalking", isWalking);
    }

}


