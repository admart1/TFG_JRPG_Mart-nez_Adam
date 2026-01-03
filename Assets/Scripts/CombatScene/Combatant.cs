using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    [Header("Datos de origen")]
    public CharacterModel PlayerSource;
    public EnemyData EnemySource;       

    [Header("Stats de combate")]
    public int MaxHP;
    public int CurrentHP;
    public int Offense;
    public int Defense;
    public int Speed;

    [Header("Actions system")]
    public int maxActionPoints = 3;
    public int CurrentActionPoints { get; private set; }
    public List<CombatAction> plannedActions = new();

    public int AttackBuff { get; private set; }

    [Header("Estado")]
    public bool IsAlive => CurrentHP > 0;
    public bool isDead = false;

    [Header("Tipo de unidad")]
    [HideInInspector] public CombatantType CombatantType;

    [Header("Sprite")]
    public SpriteRenderer spriteRenderer;

    #region INITIALIZES

    public void Initialize(CharacterModel model)
    {
        PlayerSource = model;
        EnemySource = null;
        CombatantType = CombatantType.Player;
        MaxHP = model.GetFinalStats().maxHP;
        CurrentHP = model.currentHP;
        Speed = model.GetFinalStats().speed;
        Offense = model.GetFinalStats().offense;
        Defense = model.GetFinalStats().defense;
        CurrentActionPoints = maxActionPoints;
        SetupSprite(model.definition.combatSprite);
    }

    public void Initialize(EnemyData enemy)
    {
        EnemySource = enemy;
        PlayerSource = null;
        CombatantType = CombatantType.Enemy;
        MaxHP = enemy.maxHP;
        CurrentHP = MaxHP;
        Speed = enemy.speed;
        Offense = enemy.offense;
        Defense = enemy.defense;
        CurrentActionPoints = maxActionPoints;
        SetupSprite(enemy.combatSprite);
    }

    #endregion

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            OnDeath();
        }
    }

    private void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        // notificar turnManager
        var turnManager = Object.FindFirstObjectByType<TurnManager>();
        if (turnManager != null)
            turnManager.RemoveCombatant(this);

        // log
        var deathLog = Object.FindFirstObjectByType<DeathLogController>();
        if (deathLog != null)
            deathLog.Add($"{GetDisplayName()} ha muerto");

        // quitar sprite
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    public int GetSpeed()
    {
        if (CombatantType == CombatantType.Player)
            return PlayerSource.GetFinalStats().speed;

        return EnemySource.speed;
    }

    public void ResetActionPoints()
    {
        CurrentActionPoints = maxActionPoints;
    }

    #region ACTIONS MANAGEAMENT

    public bool CanAddAction(CombatAction action)
    {
        return CurrentActionPoints >= action.Cost;
    }

    public void AddAction(CombatAction action)
    {
        if (!CanAddAction(action)) return;

        plannedActions.Add(action);
        CurrentActionPoints -= action.Cost;
    }

    public IReadOnlyList<CombatAction> GetPlannedActions()
    {
        return plannedActions;
    }

    public void ExecutePlannedActions()
    {
        foreach (var action in plannedActions)
        {
            action.Execute();
        }

        plannedActions.Clear();
    }

    #endregion  

    public string GetDisplayName()
    {
        if (CombatantType == CombatantType.Player && PlayerSource != null)
            return PlayerSource.definition.displayName;

        if (CombatantType == CombatantType.Enemy && EnemySource != null)
            return EnemySource.enemyName;

        return name;
    }

    public void SetupSprite(Sprite sprite)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;
    }

    public void AddAttackBuff(int amount)
    {
        AttackBuff += amount;
    }

    public float GetOffense()
    {
        return Offense + AttackBuff;
    }

}