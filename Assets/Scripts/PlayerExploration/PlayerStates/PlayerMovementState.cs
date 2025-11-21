using UnityEngine;

public class PlayerMovementState : PlayerState
{
    private Vector2 rawInput;
    public override void Enter()
    {
        Debug.Log("movement activo");
    }

    public override void HandleInput()
    {
        if (player.input == null) return;

        rawInput = player.input.Move.normalized;
        player.MoveDirection = rawInput.normalized;

        if (player.input.AttackPressed)
        {
            stateMachine.ChangeState(player.AttackState);
            return;
        }
        
        if (player.input.Move == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }

    }

    public override void PhysicsUpdate()
    {
        player.Rigidbody.linearVelocity = player.MoveDirection * player.MovementSpeed;
    }

    public override void LogicUpdate()
    {
        //actualizar dirección
        player.playerFacing.UpdateFacingDirection(rawInput);

        //reproducir animacion
        player.animationController.PlayAnimation(
            PlayerAnimationDatabase.PlayerState.Movement,
            player.playerFacing.facingDirection
        );
    }
}