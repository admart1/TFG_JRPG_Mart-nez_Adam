using UnityEngine;
public struct DamageResult
{
    public float damage;
    public bool isCrit;
    public bool isWeak;
    public bool isResist;
}
public static class CombatUtils
{
    public static bool RollCritical()
    {
        return Random.value < CombatConstants.CRIT_CHANCE;
    }

    public static float GetAffinityMultiplier(EnemyData enemy, SwordType swordType)
    {
        float multiplier = 1f;

        // debilidad
        if ((swordType == SwordType.Fire && enemy.weakness == EnemyWeakness.Fire) ||
            (swordType == SwordType.Ice && enemy.weakness == EnemyWeakness.Ice))
        {
            multiplier *= 1.5f;
        }

        // resistance
        if ((swordType == SwordType.Fire && enemy.resistance == EnemyResistance.Fire) ||
            (swordType == SwordType.Ice && enemy.resistance == EnemyResistance.Ice) ||
            (swordType == SwordType.Standard && enemy.resistance == EnemyResistance.Standard))
        {
            multiplier *= 0.5f;
        }

        return multiplier;
    }

    public static float RandomFactor()
    {
        return Random.Range(0.95f, 1.05f);
    }

    public static DamageResult CalculateDamage(Combatant attacker, Combatant target)
    {
        DamageResult result = new DamageResult();

        var sword = attacker.PlayerSource.GetActiveSword();

        float baseDamage =
            ((float)attacker.GetOffense() / (float)target.Defense)
            * CombatConstants.DAMAGE_MULTIPLIER;

        // afinidad
        float affinity = GetAffinityMultiplier(target.EnemySource, sword.swordType);

        result.isWeak = affinity > 1f;
        result.isResist = affinity < 1f;

        // crit
        result.isCrit = RollCritical();
        float critMultiplier = result.isCrit
            ? CombatConstants.CRIT_MULTIPLIER
            : 1f;

        float randomFactor = Random.Range(0.95f, 1.05f);

        float finalDamage =
            baseDamage * affinity * critMultiplier * randomFactor;

        result.damage = finalDamage;

        return result;
    }


}
