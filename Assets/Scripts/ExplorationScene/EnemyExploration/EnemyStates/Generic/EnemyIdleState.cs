using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private float idleTimer;
    private float idleDuration;
    private float distToDetect = 1.5f;

    public override void Enter()
    {
        idleDuration = Random.Range(0.5f, 1.5f);
        idleTimer = 0f;

        enemy.SetVelocity(Vector2.zero);
        enemy.animController?.PlayIdle();
    }

    public override void LogicUpdate()
    {
        idleTimer += Time.deltaTime;

        if (enemy.player != null && enemy.HasLineOfSight())
        {
            float dist = Vector2.Distance(enemy.transform.position, enemy.player.position);

            if (dist < distToDetect)
            {
                stateMachine.ChangeState(enemy.ChaseState); 
                return;
            }
        }

        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;

            // fin del idle, patrol?
        }
    }
}
