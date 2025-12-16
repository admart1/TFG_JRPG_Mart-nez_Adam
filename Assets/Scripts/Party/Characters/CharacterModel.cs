using UnityEngine;

public enum ActiveSword
{
    Slot1,
    Slot2
}

[System.Serializable]
public class CharacterModel
{
    // ================================
    // DATOS ESTÁTICOS
    [Header("Personaje (character definition")]
    public CharacterDefinition definition;

    // ================================
    // DATOS DINÁMICOS
    [Header("Estado actual")]
    public int level = 1;                // Nivel actual
    public int currentHP;
    public int currentMana;

    [Header("Espadas")]
    public EquipableSword SwordSlot1;
    public EquipableSword SwordSlot2;
    public ActiveSword activeSword = ActiveSword.Slot1;


    #region CONSTRUCTORES
    public void InitializeCharacter(CharacterDefinition def)
    {
        definition = def;

        // estadp
        level = def.startingLevel;
        currentHP = def.startingHP;
        currentMana = def.startingMana;

        // espadas
        SwordSlot1 = def.startingSword1;
        SwordSlot2 = def.startingSword2;
        activeSword = def.activeSword;
    }
    #endregion


    #region MÉTODOS ESTADÍSTICAS

    // devuelve los FinalStats del personaje, aplicando base, progresión por nivel y la espada activa.
    public FinalStats GetFinalStats()
    {
        if (definition == null)
        {
            Debug.LogWarning("CharacterModel no tiene def");
            return new FinalStats();
        }

        // 1. stats base
        FinalStats stats = definition.baseStats.ToStats();

        // 2. progresión nivel
        if (definition.levelProgression != null)
        {
            StatsModifier levelModifier = definition.levelProgression.GetModifierForLevel(level);
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
    #endregion


    #region MÉTODOS ESPADAS
    public EquipableSword GetActiveSword()
    {
        return activeSword == ActiveSword.Slot1 ? SwordSlot1 : SwordSlot2;
    }

    public void SwitchActiveSword()
    {
        activeSword = activeSword == ActiveSword.Slot1 ? ActiveSword.Slot2 : ActiveSword.Slot1;
    }

    public void SetActiveSword(int slot)
    {
        if (slot == 1 && activeSword != ActiveSword.Slot1)
        {
            activeSword = ActiveSword.Slot1;
        }
        else if (slot == 2 && activeSword != ActiveSword.Slot2)
        {
            activeSword = ActiveSword.Slot2;
        }
    }

public void EquipSword(EquipableSword sword, int slot)
{
    if (slot == 1 && SwordSlot2 == sword)
        SwordSlot2 = definition.emptySword;
    else if (slot == 2 && SwordSlot1 == sword)
        SwordSlot1 = definition.emptySword;

    if (slot == 1) SwordSlot1 = sword;
    else if (slot == 2) SwordSlot2 = sword;
}


    #endregion
}