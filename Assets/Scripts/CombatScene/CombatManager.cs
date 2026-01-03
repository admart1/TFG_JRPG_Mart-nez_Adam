using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CombatManager : MonoBehaviour
{
    [Header("Combat Context")]
    public CombatAdvantage combatAdvantage;

    [Header("Referencias")]
    public TurnManager turnManager;

    [Header("Prefabs")]
    [SerializeField] private Combatant playerCombatantPrefab;
    [SerializeField] private Combatant enemyCombatantPrefab;

    [Header("Spawns")]
    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Combatant lists")]
    public List<Combatant> playerCombatants = new();
    public List<Combatant> enemyCombatants = new();
    public List<Combatant> allCombatants = new();

    public bool combatEnding = false;
    public bool IsEndingCombat => combatEnding;

    public event Action OnCombatStarted;

    public enum CombatResult
{
    Victory,
    Defeat,
    Escape
}

    #region COMBAT START

    private void Start()
    {
        combatAdvantage = GameSession.Instance.CurrentCombatAdvantage;

        CreatePlayerCombatants();
        CreateEnemyCombatants();

        ApplyCombatAdvantage();

        CombatFadeTransition fade = FindFirstObjectByType<CombatFadeTransition>();

        if (fade != null)
        {
            fade.StartFadeIn(); 
            StartCoroutine(DelayedInit(fade.fadeDuration + 2f));
        }
        else
        {
            InitializeCombatants();
        }
    }

    private IEnumerator DelayedInit(float delay)
    {
        yield return new WaitForSeconds(delay);
        InitializeCombatants();
    }

    private void InitializeCombatants()
    {
        turnManager.InitializeTurns(allCombatants);
        OnCombatStarted?.Invoke();
    }

    #endregion

    #region CREATE COMBATANTS

    private void CreatePlayerCombatants()
    {
        var party = GameSession.Instance.PlayerParty;

        for (int i = 0; i < party.Count && i < playerSpawnPoints.Length; i++)
        {
            Combatant c = Instantiate(
                playerCombatantPrefab,
                playerSpawnPoints[i].position,
                Quaternion.identity,
                playerSpawnPoints[i]
            );

            c.Initialize(party[i]);
            playerCombatants.Add(c);
            allCombatants.Add(c);
        }
    }

    private void CreateEnemyCombatants()
    {
        var enemyGroup = GameSession.Instance.CurrentEnemyGroup;

        for (int i = 0; i < enemyGroup.enemies.Count && i < enemySpawnPoints.Length; i++)
        {
            Combatant c = Instantiate(
                enemyCombatantPrefab,
                enemySpawnPoints[i].position,
                Quaternion.identity,
                enemySpawnPoints[i]
            );

            c.Initialize(enemyGroup.enemies[i]);
            enemyCombatants.Add(c);
            allCombatants.Add(c);
        }
    }

    #endregion

    #region ENDBATTLE

    public void CheckCombatEnd()
    {
        if (playerCombatants.All(c => !c.IsAlive))
        {
            OnCombatEnd(CombatResult.Defeat);
            return;
        }

        if (enemyCombatants.All(c => !c.IsAlive))
        {
            OnCombatEnd(CombatResult.Victory);
            return;
        }
    }

    public void OnCombatEnd(CombatResult result)
    {
        if (combatEnding) return;
        combatEnding = true;

        StartCoroutine(EndCombatRoutine(result));
    }

    private IEnumerator EndCombatRoutine(CombatResult result)
    {
        var log = UnityEngine.Object.FindFirstObjectByType<CombatLogController>();
        
        if(result == CombatResult.Defeat)
            log?.Add("Derrota...");

        if (result == CombatResult.Victory)
            log?.Add("¡Victoria!");

        if (result == CombatResult.Escape)
            log?.Add("Huyendo...");

        yield return new WaitForSeconds(1.5f);

        ResetCombat();

        GameSession.Instance.ResolveCombatResult(result);
    }



    public void ResetCombat()
    {
        foreach (var c in playerCombatants)
            c.ResetActionPoints();
        foreach (var c in enemyCombatants)
            c.ResetActionPoints();

        foreach (var c in playerCombatants)
            c.plannedActions.Clear();
        foreach (var c in enemyCombatants)
            c.plannedActions.Clear();

        var combatLog = UnityEngine.Object.FindFirstObjectByType<CombatLogController>();
        if (combatLog != null)
            combatLog.Clear();

        var deathLog = UnityEngine.Object.FindFirstObjectByType<DeathLogController>();
        if (deathLog != null)
            deathLog.Clear();

        turnManager.ResetTurns();

        foreach (var combatant in playerCombatants)
        {
            if (combatant.PlayerSource == null) continue;

            combatant.PlayerSource.currentHP = combatant.CurrentHP;
        }

        foreach (var c in playerCombatants)
            Destroy(c.gameObject);
        foreach (var c in enemyCombatants)
            Destroy(c.gameObject);

        playerCombatants.Clear();
        enemyCombatants.Clear();
        allCombatants.Clear();

        GameSession.Instance.EndCombat();
    }

    private void ApplyCombatAdvantage()
    {
        if (combatAdvantage == CombatAdvantage.Neutral)
            return;

        var log = UnityEngine.Object.FindFirstObjectByType<CombatLogController>();

        if (combatAdvantage == CombatAdvantage.PlayerAdvantage)
        {
            log?.Add("¡Ventaja inicial!");
            log?.Add("El ataque del grupo ha subido.");

            foreach (var player in playerCombatants)
                player.AddAttackBuff(5); // ajusta número
        }
        else if (combatAdvantage == CombatAdvantage.EnemyAdvantage)
        {
            log?.Add("¡Emboscada enemiga!");
            log?.Add("El ataque de los enemigos ha augmentado");

            foreach (var enemy in enemyCombatants)
                enemy.AddAttackBuff(5);
        }
    }

    #endregion
}