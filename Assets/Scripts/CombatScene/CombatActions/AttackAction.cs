using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackAction : CombatAction
{
    public AttackAction(Combatant caster, Combatant target)
        : base(caster, target, 1) { }

    public override void Execute()
    {
        if (Target == null) return;
        switch (Target.CombatantType)
        {
            case CombatantType.Player:
                float  damageToPlayer = ((float)Caster.GetOffense() / (float)Target.Defense) * CombatConstants.DAMAGE_MULTIPLIER;

                Target.TakeDamage(Mathf.RoundToInt(damageToPlayer));

                var logPlayer = Object.FindFirstObjectByType<CombatLogController>();
                if (logPlayer != null)
                {
                    logPlayer.Add($"{Caster.GetDisplayName()} ataca a {Target.GetDisplayName()} Daño: {Mathf.RoundToInt(damageToPlayer)}");
                }
                break;

            case CombatantType.Enemy:
                if (Target.EnemySource != null)
                {
                    var sword = Caster.PlayerSource.GetActiveSword();
                    DamageResult result = CombatUtils.CalculateDamage(Caster, Target);

                    Target.TakeDamage(
                        Mathf.RoundToInt(result.damage)
                    );

                    var log = Object.FindFirstObjectByType<CombatLogController>();
                    if (log != null)
                    {
                        log.LogAttack(
                             Caster.GetDisplayName(),
                             Target.GetDisplayName(),
                             result
                            );
                    }
                }
                else
                {
                    Debug.LogWarning($"Enemy Combatant {Target.name} no tiene instancia de enemigo asignada.");
                }
                break;
        }
    }
}
