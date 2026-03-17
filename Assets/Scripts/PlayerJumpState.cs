using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerMovement player) : base(player) {}

    public override void Enter()
    {
        base.Enter();

        animator.SetBool("isJumping", true);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);
        JumpPressed = false;
        JumpReleased = false;
    }

    public override void Update()
    {
        base.Update();

        if (player.isGrounded && rb.linearVelocity.y <= 0.01f)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.ApplyVariableGravity();

        // Jump cut: if player releases jump while still rising, reduce upward velocity.
        if (JumpReleased && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * player.jumpCutMultiplier);
            JumpReleased = false;
        }

        float speed = player.walkSpeed;
        rb.linearVelocity = new Vector2(speed * player.facingDirection, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        animator.SetBool("isJumping", false);
    }
}