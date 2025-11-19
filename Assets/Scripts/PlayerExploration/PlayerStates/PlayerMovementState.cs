using UnityEngine;

public class PlayerMovementState : PlayerState
{
    public override void Enter()
    {
        Debug.Log("movement activo");
    }

    public override void HandleInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.AttackState);
        }
        else if (input == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else
        {
            player.MoveDirection = input.normalized;
        }
    }

    public override void PhysicsUpdate()
    {
        player.Rigidbody.linearVelocity = player.MoveDirection * player.MovementSpeed;
    }

    public override void LogicUpdate()
    {
        //actualizar dirección
        player.playerFacing.UpdateFacingDirection(player.MoveDirection);

        //reproducir animacion
        player.animationController.PlayAnimation(
            PlayerAnimationDatabase.PlayerState.Movement,
            player.playerFacing.facingDirection
        );
    }
}