using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerInput;

    [Header("Movement")]
    [SerializeField, Range(0f, 50f)] private float speed = 5f;

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
    [SerializeField] private int facingDirection = 1;

    // Input state
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpReleased;

    // Runtime state
    private bool isGrounded;

    private void Reset()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!playerInput) playerInput = GetComponent<PlayerInput>();
        if (!groundCheck)
        {
            // Try to find a child named "GroundCheck" if not manually assigned
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
    }

    private void FixedUpdate()
    {
        UpdateGroundedState();
        ApplyVariableGravity();
        ApplyMovement();
        ApplyJump();
    }

    private void ApplyMovement()
    {
        float targetSpeed = moveInput.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void ApplyJump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            jumpReleased = false;
        }

        if (jumpReleased)
        {
            // Cut jump height if the player releases the button while still rising
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * jumpCutMultiplier
                );
            }

            jumpReleased = false;
        }
    }

    private void ApplyVariableGravity()
    {
        float verticalSpeed = rb.linearVelocity.y;

        if (verticalSpeed < -0.1f)
        {
            // Falling
            rb.gravityScale = fallGravity;
        }
        else if (verticalSpeed > 0.1f)
        {
            // Rising
            rb.gravityScale = jumpGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }
    }

    private void UpdateGroundedState()
    {
        if (!groundCheck)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

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

    #region Input Callbacks

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
            jumpReleased = false;
        }
        else
        {
            // Button released
            jumpReleased = true;
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
