public class SlimeEnemy : ExplorationEnemyBase
{
    protected override void Awake()
    {
        base.Awake();

        // crear estados
        IdleState = new EnemyIdleState();
        ChaseState = new EnemyChaseState();
        AttackState = new SlimeAttackState();
        RecoveryState = new RecoveryState(0.5f);

        // inicializar estados
        IdleState.Initialize(this, stateMachine);
        ChaseState.Initialize(this, stateMachine);
        AttackState.Initialize(this, stateMachine);
        RecoveryState.Initialize(this, stateMachine);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(IdleState);
    }
}

// magic numbers..