using UnityEngine;
using System.Collections;

public class EnemyKnockbackState : EnemyState
{
    private Vector2 dir;
    private float speed = 2f;
    private float duration = 0.03f;
    private float timer;

    public override void Enter()
    {
        dir = getDirection();
        timer = 0;
        enemy.animController.PlayHurt();

        
    }

    public override void PhysicsUpdate()
    {
        timer += Time.fixedDeltaTime;

        enemy.rb.linearVelocity = dir * speed;

        if (timer >= duration)
        {
            enemy.StartCoroutine(WaitAfterKnockback());
        }
    }

    private Vector2 getDirection()
    {

        switch (enemy.playerFacing.facingDirection)
        {
            case PlayerFacing.FacingDirection.North: return new Vector2(0, 1).normalized;
            case PlayerFacing.FacingDirection.NorthEast: return new Vector2(-1, -1).normalized;
            case PlayerFacing.FacingDirection.East: return new Vector2(1, 0);
            case PlayerFacing.FacingDirection.SouthEast: return new Vector2(-1, 1).normalized;
            case PlayerFacing.FacingDirection.South: return new Vector2(0, -1).normalized;
            case PlayerFacing.FacingDirection.SouthWest: return new Vector2(1, 1).normalized;
            case PlayerFacing.FacingDirection.West: return new Vector2(-1, 0);
            case PlayerFacing.FacingDirection.NorthWest: return new Vector2(1, -1).normalized;
            default: return Vector2.zero;
        }
    }

    private IEnumerator WaitAfterKnockback()
    {
        enemy.SetVelocity(Vector2.zero);
        yield return new WaitForSeconds(0.5f);
        stateMachine.ChangeState(enemy.IdleState);
    }
}
