using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private float attackDistance = 1f;
    private float moveSpeed = 2f;
    public float maxAggroDistance = 3f;
    public Vector2 targetPos; 

    public override void Enter()
    {
        enemy.animController?.PlayMovement();
    }

    public override void LogicUpdate()
    {
        // HACIA IDLE
        if (enemy.player == null)
        {
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        bool seesPlayer = enemy.HasLineOfSight();

        if (seesPlayer)
        {
            enemy.lastSeenPlayerPosition = enemy.player.position;
            enemy.chaseMemoryTimer = enemy.chaseMemoryTime;
        }
        else
        {
            enemy.chaseMemoryTimer -= Time.deltaTime;

            if (enemy.chaseMemoryTimer <= 0f)
            {
                stateMachine.ChangeState(enemy.IdleState);
                return;
            }
        }

        targetPos = seesPlayer ? (Vector2)enemy.player.position : enemy.lastSeenPlayerPosition;
        float distance = Vector2.Distance(enemy.transform.position, targetPos);

        if (distance > maxAggroDistance)
        {
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        // HACIA ATTACK

        if (distance <= attackDistance && seesPlayer)
        {
            stateMachine.ChangeState(enemy.AttackState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        // EXCEPCIONES
        if (enemy.player == null) return;

        // COMPORTAMIENTO
        Vector2 dir = (targetPos - (Vector2)enemy.transform.position).normalized;
        enemy.SetVelocity(dir * moveSpeed);

        enemy.HandleFlip(dir);
        enemy.SetVelocity(dir * moveSpeed);
    }
}
