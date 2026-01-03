using System.Collections;
using UnityEngine;

public class EnemyStunState : EnemyState
{
    private float stunDuration = 3f;
    private float timer;

    public override void Enter()
    {
        enemy.stateMachine.isStunned = true;

        timer = 0f;
        enemy.animController?.PlayStun();
        enemy.rb.linearVelocity = Vector2.zero; 
    }

    public override void PhysicsUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= stunDuration)
         {
             Recover();
         }
    }

    private void Recover()
    {
        enemy.currentHealth = enemy.maxHealth;
        enemy.UpdateHealthBar();
        enemy.stateMachine.isStunned = false;

        stateMachine.ChangeState(enemy.RecoveryState);
    }
}
