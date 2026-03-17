using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public PlayerState currentState;

    public PlayerIdleState idleState;
    public PlayerJumpState jumpState;
    public PlayerMoveState moveState;

    [Header("References")]
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] public Animator animator;

    [Header("Movement")]
    [SerializeField, Range(0f, 50f)] public float walkSpeed = 5f;

    [Header("Jump")]
    [SerializeField] public float jumpForce = 10f;
    [SerializeField, Range(0f, 1f)] public float jumpCutMultiplier = 0.5f;

    [Header("Gravity")]
    [SerializeField] private float normalGravity = 1f;
    [SerializeField] private float fallGravity = 2f;
    [SerializeField] private float jumpGravity = 0.75f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Facing")]
    [SerializeField] public float facingDirection = 1f;

    [Header("Equipment")]
    [SerializeField] private GameObject candlePrefab;
    [SerializeField] private Transform candleAttachPoint;

    // Input state
    public Vector2 moveInput;
    public bool jumpPressed;
    public bool jumpReleased;

    //Runtime state 
    public bool isGrounded;


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

    private void Awake()
    {
        idleState = new PlayerIdleState(this);
        jumpState = new PlayerJumpState(this);
        moveState = new PlayerMoveState(this);
    }

    private void Start()
    {
        rb.gravityScale = normalGravity;

        SpawnCandle();

        ChangeState(idleState);
    }

    private void Update()
    {
        currentState.Update();

        UpdateFacing();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate();
        UpdateGroundedState();
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    private void SpawnCandle()
    {
        if (!candlePrefab || !candleAttachPoint)
        {
            return;
        }

        Instantiate(candlePrefab, candleAttachPoint.position, candleAttachPoint.rotation, candleAttachPoint);
    }


    // PLAYER'S WALKING MECHANICS
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
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

    public void ApplyVariableGravity()
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
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y); 
    }

}


