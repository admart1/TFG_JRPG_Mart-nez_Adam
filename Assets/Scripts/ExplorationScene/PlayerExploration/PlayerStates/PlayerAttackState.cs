using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private int comboNumber = 0;
    private bool bufferedAttack = false;
    private bool canChain = false;

    private float comboResetTime = 0.6f;
    private float comboTimer;

    private bool bufferedDash = false;
    public override void Enter()
    {
        player.Rigidbody.linearVelocity = Vector2.zero;

        bufferedAttack = false;
        canChain = false;

        if (Time.time > comboTimer)
            comboNumber = 0;

        comboTimer = Time.time + comboResetTime;

        PlayCurrentComboAnimation();
    }

    public override void HandleInput()
    {
        if (canChain && player.input.AttackPressed)
        {
            bufferedAttack = true;
            bufferedDash = false;
        }

        if (canChain && player.input.DashPressed && player.DashState.CanDash() && player.input.Move != Vector2.zero)
        {
            bufferedDash = true;
            bufferedAttack = false;

            player.MoveDirection = player.input.Move.normalized;
        }
    }

    public override void PhysicsUpdate()
    {

    }

    public override void LogicUpdate()
    {

    }

    public override void OnAnimationEvent(string eventName)
    {
        if (eventName == "AllowChainAttack")                                // eventos ahora en AnimationEventRelay y no en el playercontroller****
        {
            canChain = true;
        }

        if (eventName == "AttackFinished")
        {
            if (bufferedDash)
            {
                bufferedDash = false;
                player.DashState.SetDashLevel(player.currentDashLevel);
                stateMachine.ChangeState(player.DashState);
            }
            else if (bufferedAttack)
            {
                comboNumber = (comboNumber + 1) % 3;
                stateMachine.ChangeState(player.AttackState);
            }
            else
            {
                comboNumber = 0;
                stateMachine.ChangeState(player.IdleState);
            }
        }

        if (eventName == "EnableHitbox")
        {
            player.attackHitbox.EnableHitbox();
        }

        if (eventName == "DisableHitbox")
        {
            player.attackHitbox.DisableHitbox();
        }
    }
    
    private void PlayCurrentComboAnimation()
    {
        switch (comboNumber)
        {
            case 0:
                player.animationController.PlayAnimation(
                    "Attack1",
                    player.playerFacing.facingDirection
                );
                break;
            case 1:
                player.animationController.PlayAnimation(
                    "Attack2",
                    player.playerFacing.facingDirection
                );
                break;
            case 2:
                player.animationController.PlayAnimation(
                    "Attack3",
                    player.playerFacing.facingDirection
                );
                break;
        }
    }
}