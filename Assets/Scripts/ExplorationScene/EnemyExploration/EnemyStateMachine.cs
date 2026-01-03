using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }
    public bool isStunned = false;

    public void Initialize(EnemyState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        if (CurrentState != null)
            CurrentState.Exit();

        if (isStunned)
            return;

        CurrentState = newState;
        CurrentState.Enter();
    }
}
