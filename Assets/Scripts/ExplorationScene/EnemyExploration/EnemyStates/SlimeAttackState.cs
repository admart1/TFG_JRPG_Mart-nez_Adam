using UnityEngine;
using System.Collections;

public class SlimeAttackState : EnemyState
{
    private bool hasAttacked = false;
    private Coroutine attackRoutine;

    [Header("Attack Settings")]
    public float attackRange = 1f;       
    public float attackDamage = 1f;
    public float attackDashForce = 3f;
    public float attackDuration = 0.5f;    
    public float attackStunTime = 0.25f;
    public float maxAggroDistance = 5f;    

    public override void Enter()
    {
        hasAttacked = false;

        enemy.SetVelocity(Vector2.zero);

        enemy.animController?.PlayAttack();

        enemy.animController.OnAnimationEvent += HandleAnimationEvent;

        attackRoutine = enemy.StartCoroutine(AttackDash());
    }

    private IEnumerator AttackDash()
    {
        yield return new WaitForSeconds(0.1f);

        if (enemy.player == null)
            yield break;

        Vector2 direction = (enemy.player.position - enemy.transform.position).normalized;
        float timer = 0f;

        while (timer < attackDuration)
        {
            if (Vector2.Distance(enemy.transform.position, enemy.player.position) > maxAggroDistance)
                break;

            enemy.rb.linearVelocity = (enemy.player.position - enemy.transform.position).normalized * attackDashForce;

            timer += Time.deltaTime;
            yield return null;
        }

        OnAttackFinished();
    }

    private void HandleAnimationEvent(string eventName)
    {
        if (eventName == "Hit" && !hasAttacked)
        {
            hasAttacked = true;
        }

        if (eventName == "AttackFinished")
        {
            OnAttackFinished();
        }
    }

    private void OnAttackFinished()
    {
        if (attackRoutine != null)
        {
            enemy.StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        enemy.animController.OnAnimationEvent -= HandleAnimationEvent;
        enemy.SetVelocity(Vector2.zero);

        stateMachine.ChangeState(enemy.RecoveryState);
    }

    public override void Exit()
    {
        if (attackRoutine != null)
        {
            enemy.StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        enemy.animController.OnAnimationEvent -= HandleAnimationEvent;
        enemy.SetVelocity(Vector2.zero);
    }
}
