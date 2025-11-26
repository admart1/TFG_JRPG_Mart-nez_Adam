using UnityEngine;

public class PlayerMovementState : PlayerState
{
    private Vector2 rawInput;
    public override void Enter()
    {

    }

    public override void HandleInput()
    {
        if (player.input == null) return;

        rawInput = player.input.Move.normalized; // raw para el facingdirection y normalizado para el movimiento
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

        if (player.input.DashPressed && player.DashState.CanDash())
        {
            player.DashState.SetDashLevel(player.currentDashLevel);
            stateMachine.ChangeState(player.DashState);
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
            "Movement",
            player.playerFacing.facingDirection
        );
    }
}