using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public override void Enter()
    {
        player.MovementSpeed = player.BaseSpeed;
        player.Rigidbody.linearVelocity = Vector2.zero;
    }

    public override void HandleInput()
    {
        if (player.input.ToBattlePressed && player.combatDetector.SelectedEnemy != null)
        {
            CombatTransitionManager.Instance.StartCombat(
              player.gameObject,
              player.combatDetector.SelectedEnemy,
              CombatAdvantage.Neutral
            );
        }

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

        if (player.input.SwordMenuPressed)
        {
            stateMachine.ChangeState(player.SwordMenuState);
        }

        if (player.input.SwapSwordsPressed)
        {
            player.character.SwitchActiveSword();
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
