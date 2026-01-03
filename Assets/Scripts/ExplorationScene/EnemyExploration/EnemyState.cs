using UnityEngine;

public abstract class EnemyState
{
    protected ExplorationEnemyBase enemy;
    protected EnemyStateMachine stateMachine;
    public virtual void OnCollisionEnter(Collision2D collision) { }

    public void Initialize(ExplorationEnemyBase enemy, EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void OnAnimationEvent(string eventName) { }
}
