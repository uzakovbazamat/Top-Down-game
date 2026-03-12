using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerInput;

    [Header("Movement")]
    [SerializeField, Range(0f, 50f)] private float speed = 5f;

    [Header("Facing")]
    [SerializeField] private int facingDirection = 1;

    private Vector2 moveInput;

    private void Reset()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!playerInput) playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Flip();
    }

    private void FixedUpdate()
    {
        float targetSpeed = moveInput.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        if (moveInput.x > 0.1f)
            facingDirection = 1;
        else if (moveInput.x < -0.1f)
            facingDirection = -1;

        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
