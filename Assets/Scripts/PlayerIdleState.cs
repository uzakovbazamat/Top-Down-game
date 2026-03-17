using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        animator.SetBool("isIdle", true);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (JumpPressed)
        {
            JumpPressed = false;
            player.ChangeState(player.jumpState);
        }
        else if (Mathf.Abs(MoveInput.x) > .1f)
        {
            player.ChangeState(player.moveState);
        }
    }

    public override void Exit()
    {
        animator.SetBool("isIdle", false);
    }
}
