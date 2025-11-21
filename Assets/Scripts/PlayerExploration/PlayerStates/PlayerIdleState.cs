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

        if (player.input.Move != Vector2.zero)
        {
            stateMachine.ChangeState(player.MovementState);
        }

        if (player.input.AttackPressed)
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
