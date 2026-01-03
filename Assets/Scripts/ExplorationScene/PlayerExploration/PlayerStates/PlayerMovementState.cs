using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements.Experimental;

public class PlayerMovementState : PlayerState
{
    private Vector2 rawInput;
    public override void HandleInput()
    {
        if (player.input == null) return;

        rawInput = player.input.Move.normalized; // raw para el facingdirection y normalizado para el movimiento
        player.MoveDirection = rawInput;

        if (player.input.ToBattlePressed && player.combatDetector.SelectedEnemy != null)
        {
            var enemy = player.combatDetector.SelectedEnemy;
            var advantage = enemy.GetCombatAdvantageAgainstPlayer();

            CombatTransitionManager.Instance.StartCombat(enemy, advantage);
        }

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

        if (player.input.SwordMenuPressed)
        {
            stateMachine.ChangeState(player.SwordMenuState);
        }

        if (player.input.SwapSwordsPressed)
        {
            player.character.SwitchActiveSword();
        }
    }

    public override void PhysicsUpdate()
    {
        HandleTriggersHeightChange();

        HandleLedge();

        if (player.triggerDetector.currentTrigger != null && player.triggerDetector.currentTrigger.isRamp)
        {
            HandleRampMovement();
        }
        else
        {
            player.SetVelocity(player.MoveDirection * player.MovementSpeed);
        }
    }

    public override void LogicUpdate()
    {
        player.playerFacing.UpdateFacingDirection(rawInput);

        player.animationController.PlayAnimation(
            "Movement",
            player.playerFacing.facingDirection
        );
    }

    private void HandleTriggersHeightChange()
    {
        WorldTrigger trigger = player.triggerDetector.currentTrigger;

        // simples
        if (trigger != null && trigger.simpleHeightchange)
        {
            player.heightSystem.SetHeight(trigger.targetHeight);
        }

        // rampas
      /*  if (trigger != null && trigger.isRamp)
        {
            player.heightSystem.EnterRamp();
        }*/
    }

    private void HandleRampMovement()
    {
        WorldTrigger trigger = player.triggerDetector.currentTrigger;
        if (trigger == null || !trigger.isRamp)
            return;

        Vector2 input = player.MoveDirection;

        if (Mathf.Abs(input.x) <= 0.01f)
        {
            player.SetVelocity(input * (player.MovementSpeed * trigger.rampSpeedModifier));
            return;
        }

        if (trigger.rampToRight)
        {
            input.y += input.x * trigger.rampInclination;
        }

        if (trigger.rampToLeft)
        {
            input.y -= input.x * trigger.rampInclination;
        }

        input = input.normalized;

        player.SetVelocity(input * (player.MovementSpeed * trigger.rampSpeedModifier));
    }


    private void HandleLedge()
    {
        var trigger = player.triggerDetector.currentTrigger;
        if (trigger == null || !trigger.isLedge) return;

        if (IsMovingTowardsLedge(trigger))
        {
            player.FallState.targetHeight = trigger.targetHeight;
            player.FallState.SetupFall(trigger.targetHeight, player.MoveDirection, PlayerFallState.FallOrigin.FromMovement);
            stateMachine.ChangeState(player.FallState);
        }
    }

    private bool IsMovingTowardsLedge(WorldTrigger ledge)
    {
        float x = player.MoveDirection.x;
        float y = player.MoveDirection.y;

        if (ledge.fallToEast && x > 0)
            return true;

        if (ledge.fallToWest && x < 0)
            return true;

        if (ledge.fallToNorth && y > 0)
            return true;

        if (ledge.fallToSouth && y < 0)
            return true;

        return false;
    }
}


