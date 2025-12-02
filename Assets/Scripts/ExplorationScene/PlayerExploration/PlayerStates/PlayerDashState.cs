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
                hasEndLag = false;
                hasCooldown = true;
                cooldownDuration = 0.5f;
                postDashBoost = 2f; 
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

        //player.SetInvulnerable(true);   PARA EL FUTURO.

        player.Rigidbody.linearVelocity = player.MoveDirection * dashSpeed;

        player.animationController.PlayAnimation(
            "Dash",
            player.playerFacing.facingDirection
        );
    }

    public override void HandleInput()
    {
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

                player.Rigidbody.linearVelocity = Vector2.zero;

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
        if (!endLagActive) // dash
        {
            player.Rigidbody.linearVelocity = player.MoveDirection * dashSpeed;
        }
        else                // endlag
        {
            player.Rigidbody.linearVelocity = Vector2.zero;
        }
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
}

 /* Dash implementaciones pendientes
 endlag
 invulnerabilidad (proyectiles y ataques, no enemigos)
 dash + ataque chain
 ataque + dash chain
 dash + dash chain
 curva de velocidad
 muros atravesables SOLO con dash
  */