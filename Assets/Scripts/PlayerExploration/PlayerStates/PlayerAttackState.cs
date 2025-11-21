using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private int comboNumber = 0;
    private bool bufferedAttack = false;
    private bool canChain = false;

    private float comboResetTime = 0.6f;
    private float comboTimer;
    public override void Enter()
    {
        Debug.Log("attack activo");

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
        if (eventName == "AllowChainAttack")
        {
            canChain = true;
        }

        if (eventName == "AttackFinished")
        {
            if (bufferedAttack)
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
    }

    private void PlayCurrentComboAnimation()
    {
        switch (comboNumber)
        {
            case 0:
                player.animationController.PlayAnimation(
                    PlayerAnimationDatabase.PlayerState.Attack,
                    player.playerFacing.facingDirection
                );
                break;
            case 1:
                player.animationController.PlayAnimation(
                    PlayerAnimationDatabase.PlayerState.Attack2,
                    player.playerFacing.facingDirection
                );
                break;
            case 2:
                player.animationController.PlayAnimation(
                    PlayerAnimationDatabase.PlayerState.Attack3,
                    player.playerFacing.facingDirection
                );
                break;
        }
    }
}