using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements.Experimental;

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
        HandleSimpleHeightChange();

        HandleLedge();

        if (player.triggerDetector.currentTrigger != null && player.triggerDetector.currentTrigger.isRamp)
        {
            HandleRampMovement();
        }
        else
        {
            player.Rigidbody.linearVelocity = player.MoveDirection * player.MovementSpeed;
        }
    }

    public override void LogicUpdate()
    {
        //actualizar direcci�n
        player.playerFacing.UpdateFacingDirection(rawInput);

        //reproducir animacion
        player.animationController.PlayAnimation(
            "Movement",
            player.playerFacing.facingDirection
        );
    }

    private void HandleSimpleHeightChange()
    {
        WorldTrigger trigger = player.triggerDetector.currentTrigger;

        if (trigger != null && trigger.simpleHeightchange)
        {
            player.heightSystem.SetHeight(trigger.targetHeight);
        }
    }

    private void HandleRampMovement()
    {

        WorldTrigger trigger = player.triggerDetector.currentTrigger;
        if (trigger == null || !trigger.isRamp)
            return;

        Vector2 direction = player.MoveDirection;
        PlayerFacing.FacingDirection facing = player.playerFacing.facingDirection;


        if (trigger.rampToRight)
        {
            if (facing == PlayerFacing.FacingDirection.East)
                direction = new Vector2(1, trigger.rampInclination);
            else if (facing == PlayerFacing.FacingDirection.West)
                direction = new Vector2(-1, -trigger.rampInclination);
        }

        if (trigger.rampToLeft)
        {
            if (facing == PlayerFacing.FacingDirection.East)
                direction = new Vector2(-1, -trigger.rampInclination);
            else if (facing == PlayerFacing.FacingDirection.West)
                direction = new Vector2(1, trigger.rampInclination);
        }

        player.Rigidbody.linearVelocity = direction * (player.MovementSpeed * trigger.rampSpeedModifier);

    }

    private void HandleLedge()
    {
        var trigger = player.triggerDetector.currentTrigger;
        if (trigger == null || !trigger.isLedge) return;

        if (IsMovingTowardsLedge(trigger))
        {
            player.FallState.targetHeight = trigger.targetHeight;
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


