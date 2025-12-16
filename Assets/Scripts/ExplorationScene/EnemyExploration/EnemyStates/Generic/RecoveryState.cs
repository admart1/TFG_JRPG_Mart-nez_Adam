using UnityEngine;

public class RecoveryState : EnemyState
{
    private float recoveryTime;
    private float timer;

    public RecoveryState(float recoveryDuration)
    {
        this.recoveryTime = recoveryDuration;
    }

    public override void Enter()
    {
        timer = 0f;

        enemy.SetVelocity(Vector2.zero);

        enemy.animController?.PlayRecovery();
    }

    public override void LogicUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= recoveryTime)
        {
            DecideNextAction();
        }
    }

    private void DecideNextAction()
    {
        // idle
        if (enemy.player == null)
        {
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        float dist = Vector2.Distance(enemy.transform.position, enemy.player.position);

        // ataque
        if (dist <= 1f * 1.1f)
        {
            stateMachine.ChangeState(enemy.AttackState);
            return;
        }

        // perseguir
        if (dist <= 5f)
        {
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // idle
        stateMachine.ChangeState(enemy.IdleState);
    }
}

// arreglar los numeros magicos