using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;

        Debug.Log("idle activo");
    }

    public override void HandleInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input != Vector2.zero)
        {
            stateMachine.ChangeState(player.MovementState);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.AttackState);
        }
    }

public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.animationController.PlayAnimation(
            PlayerAnimationDatabase.PlayerState.Idle,
            player.playerFacing.facingDirection
        );
    }
}
