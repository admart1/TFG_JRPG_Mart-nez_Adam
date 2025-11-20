using UnityEngine;

public enum ActiveSword
{
    Slot1,
    Slot2
}

[System.Serializable]
public class CharacterModel
{
    // DATOS
    [Header("Identidad")]
    public string characterName;
    public BaseStats baseStats;
    public int level = 1;

    [Header("Espadas")]
    public EquipableSword SwordSlot1;
    public EquipableSword SwordSlot2;
    public ActiveSword activeSword = ActiveSword.Slot1;

    [Header("Progresión por nivel")]
    public LevelProgression levelProgression;

    // devuelve los FinalStats del personaje, aplicando base, progresión por nivel y la espada activa.
    public FinalStats GetFinalStats()
    {
        // 1. Stats base
        FinalStats stats = baseStats.ToStats();

        // 2. Progresión por nivel
        if (levelProgression != null)
        {
            StatsModifier levelModifier = levelProgression.GetModifierForLevel(level);
            stats.ApplyModifier(levelModifier);
        }

        // 3. Espada activa
        EquipableSword active = GetActiveSword();
        if (active != null)
        {
            stats.ApplyModifier(active.statsModifier);
        }

        return stats;
    }

    /// devuelve la espada activa
    public EquipableSword GetActiveSword()
    {
        return activeSword == ActiveSword.Slot1 ? SwordSlot1 : SwordSlot2;
    }

    /// cambia la espada activa entre Slot1 y Slot2.
    public void SwitchActiveSword()
    {
        activeSword = activeSword == ActiveSword.Slot1 ? ActiveSword.Slot2 : ActiveSword.Slot1;
    }

    public void EquipSword(EquipableSword sword, int slot)
    {
        if (slot == 1) SwordSlot1 = sword;
        else if (slot == 2) SwordSlot2 = sword;
    }

    public void EquipSwordActive(EquipableSword sword)
    {
        if (SwordSlot1 == sword) activeSword = ActiveSword.Slot1;
        else if (SwordSlot2 == sword) activeSword = ActiveSword.Slot2;
        else
        {
            // si la espada no estaba en los slots, la ponemos en Slot1 y la activamos
            SwordSlot1 = sword;
            activeSword = ActiveSword.Slot1;
        }
    }
}
