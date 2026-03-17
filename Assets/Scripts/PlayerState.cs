using UnityEngine;

public abstract class PlayerState
{
    protected PlayerMovement player;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected float horizontalSpeed;

    protected bool JumpPressed {get => player.jumpPressed; set => player.jumpPressed = value;}
    protected bool JumpReleased {get => player.jumpReleased; set => player.jumpReleased = value;}
    protected Vector2 MoveInput => player.moveInput;
    

    public PlayerState(PlayerMovement player)
    {
        this.player = player;
        this.animator = player.animator;
        this.rb = player.rb;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}
