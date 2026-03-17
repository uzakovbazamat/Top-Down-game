using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();

        // cache current horizontal input magnitude for animation logic
        horizontalSpeed = Mathf.Abs(MoveInput.x);

        if (JumpPressed)
        {
            player.ChangeState(player.jumpState);
        }
        else if (Mathf.Abs(MoveInput.x) < .1f)
        {
            player.ChangeState(player.idleState);
        }
        else
        {
            bool isWalking = horizontalSpeed > 0.1f && player.isGrounded;
            animator.SetBool("isWalking", isWalking);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float speed = player.walkSpeed;
        rb.linearVelocity = new Vector2(speed * player.facingDirection, rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        animator.SetBool("isWalking", false);
    }
}
