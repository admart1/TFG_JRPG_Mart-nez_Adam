using UnityEngine;
using System.Collections;

public class SlimeAttackState : EnemyState
{
    private enum AttackPhase
    {
        Charging,
        Dashing,
        Finished
    }

    private bool hasAttacked = false;
    private Coroutine attackRoutine;

    [Header("Attack Settings")]
    public float chargeTime = 2f;
    public float attackRange = 1f;       
    public float attackDamage = 1f;
    public float attackDashForce = 3f;
    public float attackDuration = 0.5f;    
    public float attackStunTime = 0.25f;
    public float maxAggroDistance = 5f;
    private AttackPhase phase;
    private Vector2 attackDirection;

    private bool isHitActive;
    private bool hasHitPlayer;

    public override void Enter()
    {
        hasHitPlayer = false;
        isHitActive = false;

        enemy.SetVelocity(Vector2.zero);

        phase = AttackPhase.Charging;

        hasAttacked = false;

        enemy.animController?.PlayCharging();

        enemy.animController.OnAnimationEvent += HandleAnimationEvent;

        enemy.StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        float timer = 0f;

        while (timer < chargeTime)
        {
            if (enemy.player == null)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        StartDash();
    }

    private void StartDash()
    {
        if (enemy.player == null)
        {
            OnAttackFinished();
            return;
        }

        if (stateMachine.CurrentState == enemy.StunState)
            return;

        phase = AttackPhase.Dashing;

        attackDirection =
            ((Vector2)enemy.player.position - (Vector2)enemy.transform.position).normalized;

        enemy.HandleFlip(attackDirection);

        enemy.animController?.PlayAttack();

        attackRoutine = enemy.StartCoroutine(AttackDash());
    }

    private IEnumerator AttackDash()
    {
        float timer = 0f;

        while (timer < attackDuration)
        {
            if (stateMachine.CurrentState == enemy.StunState)
                yield break;

            enemy.rb.linearVelocity = attackDirection * attackDashForce;

            timer += Time.deltaTime;
            yield return null;
        }

        OnAttackFinished();
    }

    private void HandleAnimationEvent(string eventName)
    {
        if (eventName == "Hit")
        {
            isHitActive = true;
        }

        if (eventName == "AttackFinished")
        {
            isHitActive = false;
            OnAttackFinished();
        }
    }

    public override void OnCollisionEnter(Collision2D collision)
    {
        if (!isHitActive || hasHitPlayer)
            return;

        if (collision.collider.CompareTag("PlayerHurt"))
        {
            hasHitPlayer = true;

            Debug.Log("HIT CONFIRMADO");

            CombatTransitionManager.Instance.StartCombat(
                enemy,
                CombatAdvantage.EnemyAdvantage
            );
        }
    }

    private void OnAttackFinished()
    {
        if (stateMachine.CurrentState == enemy.StunState)
            return;

        if (phase == AttackPhase.Finished)
            return;

        phase = AttackPhase.Finished;

        if (attackRoutine != null)
            enemy.StopCoroutine(attackRoutine);

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
