using System;
using UnityEngine;

[Serializable]
public struct FinalStats
{
    public int maxHP;
    public int maxMana;
    public int offense;
    public int defense;
    public int speed;

    public FinalStats(int maxHP, int maxMana, int offense, int defense, int speed) // se llama desde basestats y crea el FinalStats con los stats del SO
    {
        this.maxHP = maxHP;
        this.maxMana = maxMana;
        this.offense = offense;
        this.defense = defense;
        this.speed = speed;
    }

    public void ApplyModifier(StatsModifier mod)                    // calcula los modificadores que recibe de el StatModifier de la Espada que lleva
    {
        maxHP += mod.hpFlat;
        maxMana += mod.manaFlat;
        offense += mod.offenseFlat;
        defense += mod.defenseFlat;
        speed += mod.speedFlat;

        maxHP = (int)(maxHP * (1f + mod.hpPercent));
        maxMana = (int)(maxMana * (1f + mod.manaPercent));
        offense = (int)(offense * (1f + mod.offensePercent));
        defense = (int)(defense * (1f + mod.defensePercent));
        speed = (int)(speed * (1f + mod.speedPercent));
    }
}