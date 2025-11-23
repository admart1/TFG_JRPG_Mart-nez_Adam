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

        if (player.input.Move != Vector2.zero)              // ir a movimiento
        {
            stateMachine.ChangeState(player.MovementState);
        }

        if (player.input.AttackPressed)                     // ir a ataque
        {
            stateMachine.ChangeState(player.AttackState);
        }

        if (player.input.ChangeCharacterPressed)            // cambiar personaje
        {
            player.partyManager.NextCharacter();
            player.character = player.partyManager.activeCharacter;
        }
    }

public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.animationController.PlayAnimation(
            "Idle",
            player.playerFacing.facingDirection
        );
    }
}
