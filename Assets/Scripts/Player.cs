using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Optional visuals")]
    [SerializeField] private Transform visualsToFlip;

    private Rigidbody2D rb;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // If you didn't assign a visual, just flip this object (the sprite on it)
        if (visualsToFlip == null)
        {
            visualsToFlip = transform;
        }
    }

    private void Update()
    {
        // Read left/right keys using the new Input System
        float input = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                input -= 1f;
            }

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                input += 1f;
            }
        }

        moveInput = Mathf.Clamp(input, -1f, 1f);

        if (visualsToFlip != null && moveInput != 0f)
        {
            Vector3 s = visualsToFlip.localScale;
            s.x = Mathf.Abs(s.x) * Mathf.Sign(moveInput);
            visualsToFlip.localScale = s;
        }
    }

    private void FixedUpdate()
    {
        // Simple movement: directly set horizontal speed based on input
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
}
