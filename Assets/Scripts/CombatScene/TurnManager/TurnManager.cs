using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    public CombatManager combatManager;

    [Header("Turns")]
    public List<Combatant> orderedCombatants = new();
    private int currentIndex = -1;

    [Header("Turno actual")]
    public Combatant currentCombatant;
    public TurnState CurrentTurnState { get; private set; }

    // eventos
    public event System.Action<Combatant> OnTurnStarted;
    public event System.Action<TurnState> OnTurnStateChanged;

    public void InitializeTurns(List<Combatant> allCombatants)  // orden de turnos inicial
    {
        orderedCombatants = new List<Combatant>(allCombatants);

        orderedCombatants.Sort((a, b) =>
        {
            int speedA = a.GetSpeed();
            int speedB = b.GetSpeed();
            return speedB.CompareTo(speedA);
        });

        currentIndex = 0;

        StartTurn();
    }

    #region BASIC TURN DYNAMIC

    private void StartTurn()
    {
        currentCombatant = orderedCombatants[currentIndex];

        currentCombatant.ResetActionPoints();

        if (currentCombatant.CombatantType == CombatantType.Player)
        {
            CurrentTurnState = TurnState.WaitingForInput;
        }
        else if (currentCombatant.CombatantType == CombatantType.Enemy)
        {
            CurrentTurnState = TurnState.ExecutingActions;
            StartCoroutine(EnemyTurnRoutine(currentCombatant));
        }

        OnTurnStarted?.Invoke(currentCombatant);
        OnTurnStateChanged?.Invoke(CurrentTurnState);
    }

    public void EndTurn()
    {
        if (CurrentTurnState == TurnState.Ending) return;

        if (combatManager == null) return;

        if (combatManager.IsEndingCombat) return;

        currentCombatant.ExecutePlannedActions();

        CurrentTurnState = TurnState.Ending;

        combatManager.CheckCombatEnd();

        if (combatManager.IsEndingCombat) return;

        AdvanceIndex();
        StartTurn();
    }

    private void AdvanceIndex()
    {
        currentIndex++;

        if (currentIndex >= orderedCombatants.Count)
        {
            currentIndex = 0;
        }
    }

    #endregion

    #region TURNOS DEL ENEMIGO supercodigoespaguetti

    private IEnumerator EnemyTurnRoutine(Combatant enemy)
    {
        yield return new WaitForSeconds(0.6f);

        var log = Object.FindFirstObjectByType<CombatLogController>();

        log?.Add($"Turno de: {enemy.GetDisplayName()}.");

        yield return new WaitForSeconds(0.4f);

        var action = ChooseEnemyAction();

        switch (action)
        {
            case EnemyAction.SingleAttack:
                yield return SingleAttack(enemy);
                break;

            case EnemyAction.AreaAttack:
                yield return AreaAttack(enemy);
                break;

            case EnemyAction.BuffAttack:
                yield return BuffAttack(enemy);
                break;
        }

        yield return new WaitForSeconds(4f);
        log?.Add($"");
        log?.Add($"");
        log?.Add($"");

        EndTurn();
    }

    private enum EnemyAction
    {
        SingleAttack,
        AreaAttack,
        BuffAttack
    }

    private EnemyAction ChooseEnemyAction()
    {
        int roll = Random.Range(0, 100);

        if (roll < 50)
            return EnemyAction.SingleAttack;
        else if (roll < 85)
            return EnemyAction.AreaAttack;
        else
            return EnemyAction.BuffAttack;
    }

    private IEnumerator SingleAttack(Combatant enemy)
    {
        var targets = combatManager.playerCombatants
            .Where(c => c.IsAlive)
            .ToList();

        if (targets.Count == 0) yield break;

        var target = targets[Random.Range(0, targets.Count)];

        var log = Object.FindFirstObjectByType<CombatLogController>();
        log?.Add($"{enemy.GetDisplayName()} ataca a {target.GetDisplayName()}");

        yield return new WaitForSeconds(0.4f);

        var action = new AttackAction(enemy, target);
        action.Execute();

        yield return new WaitForSeconds(0.6f);
    }

    private IEnumerator AreaAttack(Combatant enemy)
    {
        var targets = combatManager.playerCombatants
            .Where(c => c.IsAlive)
            .OrderBy(_ => Random.value)
            .Take(2)
            .ToList();

        if (targets.Count == 0) yield break;

        var log = Object.FindFirstObjectByType<CombatLogController>();
        log?.Add($"{enemy.GetDisplayName()} lanza un ataque en área");

        yield return new WaitForSeconds(0.4f);

        foreach (var target in targets)
        {
            var action = new AttackAction(enemy, target);
            action.Execute();

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator BuffAttack(Combatant enemy)
    {
        var log = Object.FindFirstObjectByType<CombatLogController>();

        log?.Add($"{enemy.GetDisplayName()} aumenta su ataque");

        yield return new WaitForSeconds(0.4f);

        enemy.AddAttackBuff(10);

        yield return new WaitForSeconds(0.6f);
    }

    #endregion

    public void RemoveCombatant(Combatant c)
    {
        if (combatManager.playerCombatants.Contains(c))
            combatManager.playerCombatants.Remove(c);
        if (combatManager.enemyCombatants.Contains(c))
            combatManager.enemyCombatants.Remove(c);

        if (orderedCombatants.Contains(c))
            orderedCombatants.Remove(c);
    }

    public void ResetTurns()
    {
        orderedCombatants.Clear();
        currentCombatant = null;
        currentIndex = 0;
    }
}
