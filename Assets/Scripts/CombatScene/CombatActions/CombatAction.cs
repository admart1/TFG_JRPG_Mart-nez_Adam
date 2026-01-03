using UnityEngine;

public abstract class CombatAction
{
    public Combatant Caster { get; protected set; }
    public Combatant Target { get; protected set; }
    public int Cost { get; protected set; }

    protected CombatAction(Combatant caster, Combatant target, int cost)
    {
        Caster = caster;
        Target = target;
        Cost = cost;
    }

    public abstract void Execute();
}
