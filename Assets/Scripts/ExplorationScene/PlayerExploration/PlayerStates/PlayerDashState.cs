using UnityEngine;

public enum DashLevel
{
    Level1, // si(endlag y cooldown) no(boost de vel)
    Level2, // si(cooldown) no(endlag y boost de vel) 
    Level3, // si(cooldown y boost de vel) no(endlag)
    Level4  // si(boost de vel) no(endlag y cooldown)
}

public class PlayerDashState : PlayerState
{
    // base
    private readonly float dashDuration = 0.1f;
    private readonly float dashSpeed = 12f;

    private float timer;

    // por nivel
    private DashLevel currentDashLevel;

    private bool hasEndLag;
    private bool endLagActive = false;
    private readonly float endLagDuration = 0.1f;

    private bool hasCooldown;
    private float cooldownDuration;
    private float cooldownTimer;

    private float postDashBoost;

    // buffers
    private bool bufferedAttack = false;
    private bool bufferedDash = false;
    private int bufferedFallTargetHeight = -1; // -1= no hay caida
    private bool skipFall = true;
    public WorldTrigger bufferedTrigger = null;

    #region NIVELES DEL DASH
    public void SetDashLevel(DashLevel level)
    {
        currentDashLevel = level;

        switch (level)
        {
            case DashLevel.Level1:
                hasEndLag = true;
                hasCooldown = true;
                postDashBoost = 0f;
                break;
            case DashLevel.Level2:
                hasEndLag = false;
                hasCooldown = true;
                postDashBoost = 0f;
                break;
            case DashLevel.Level3:
                hasEndLag = true;
                hasCooldown = true;
                cooldownDuration = 0.3f;
                postDashBoost = 0f; 
                break;
            case DashLevel.Level4:
                hasEndLag = false;
                hasCooldown = false;
                cooldownDuration = 0.2f;
                postDashBoost = 2f;
                break;
        }
    }
    #endregion

    public override void Enter()
    {
        player.MovementSpeed = player.BaseSpeed;

        timer = dashDuration;
        endLagActive = false;
        skipFall = true;
        bufferedFallTargetHeight = -1;

        //player.SetInvulnerable(true);   PARA EL FUTURO.

        player.SetVelocity(player.MoveDirection * dashSpeed);

        player.animationController.PlayAnimation(
            "Dash",
            player.playerFacing.facingDirection
        );
    }

    public override void HandleInput()
    {
        if (IsFallingThroughLedge())
            return;

        if (player.input.AttackPressed)
        {
            bufferedAttack = true;
            bufferedDash = false;
        }
        if (player.input.DashPressed && CanDash() && timer <= dashDuration * 0.6f)
        {
            bufferedDash = true;
            bufferedAttack = false;
        }
    }
    

    public override void LogicUpdate()
    {
        timer -= Time.deltaTime;

        if (hasEndLag)
        {
            if (timer <= 0f && !endLagActive)  // inicio del endlag
            {
                endLagActive = true;
                timer = endLagDuration;

                player.SetVelocity(Vector2.zero);

                // player.SetInvulnerable(false);   PARA EL FUTUR
            }
            else if (timer <= 0f && endLagActive)  //final del dash
            {
                ExitDash();
            }
        }
        else                                // no endlag
        {
            if (timer <= 0f)
                ExitDash();
        }
    }

    public override void PhysicsUpdate()
    {
        HandleTriggersHeightChange();

        Vector2 velocity = ComputeDashVelocity();
        player.SetVelocity(velocity);
    }

    private Vector2 ComputeDashVelocity()
    {
        if (endLagActive)
            return Vector2.zero;

        Vector2 direction = player.MoveDirection;
        var trigger = player.triggerDetector.currentTrigger;

        if (trigger != null && trigger.isRamp)
            direction = ApplyRampInclination(direction, trigger);

        return direction * dashSpeed;
    }

    private Vector2 ApplyRampInclination(Vector2 input, WorldTrigger trigger)
    {
        if (Mathf.Abs(input.x) <= 0.01f)
            return input; 

        if (trigger.rampToRight) input.y += input.x * trigger.rampInclination;
        if (trigger.rampToLeft) input.y -= input.x * trigger.rampInclination;

        return input.normalized;
    }


    private void ExitDash()
    {
        if (postDashBoost > 0f)
        {
            player.MovementSpeed += postDashBoost;
        }

        if (hasCooldown)
            cooldownTimer = cooldownDuration;


        // EXIT CON CHAIN
        if (bufferedAttack)
        {
            bufferedAttack = false;
            stateMachine.ChangeState(player.AttackState);
            return;
        }

        if (bufferedDash && CanDash())
        {
            bufferedDash = false;
            stateMachine.ChangeState(this);
            return;
        }

        // EXIT CAÍDA
        var finalTrigger = player.triggerDetector.currentTrigger;
        skipFall = finalTrigger != null && finalTrigger.dashLanding;
        if (!skipFall && bufferedFallTargetHeight != -1)
        {
            player.FallState.targetHeight = bufferedFallTargetHeight;

            player.FallState.SetupFall(bufferedFallTargetHeight, player.MoveDirection, PlayerFallState.FallOrigin.FromDash);

            bufferedFallTargetHeight = -1;

            stateMachine.ChangeState(player.FallState);
            return;
        }

        // EXIT NORMAL
        if (player.input.Move == Vector2.zero)
            stateMachine.ChangeState(player.IdleState);
        else
            stateMachine.ChangeState(player.MovementState);
    }

    public bool CanDash()
    {
        return cooldownTimer <= 0f;
    }

    public void DashTimerUpdate()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    private void HandleTriggersHeightChange()
    {
        WorldTrigger trigger = player.triggerDetector.currentTrigger;

        if (trigger == null) return;

        // simples
        if (trigger.simpleHeightchange)
        {
            player.heightSystem.SetHeight(trigger.targetHeight);
        }

        // rampas
     /*   if (trigger.isRamp)
        {
            player.heightSystem.EnterRamp();
        }*/

        if (trigger.isLedge && !trigger.dashLanding && bufferedFallTargetHeight == -1)
        {
            bufferedAttack = false;
            bufferedDash = false;

            bufferedFallTargetHeight = trigger.targetHeight;

            bufferedTrigger = player.triggerDetector.currentTrigger;

            skipFall = false; 
        }

        // dash landing
        if (trigger.dashLanding)
            skipFall = true;
    }

    private bool IsFallingThroughLedge()
    {
        var trigger = player.triggerDetector.currentTrigger;
        return trigger != null && trigger.isLedge;
    }
}

 /* Dash implementaciones pendientes
 invulnerabilidad (proyectiles y ataques, no enemigos)
 muros atravesables SOLO con dash
  */