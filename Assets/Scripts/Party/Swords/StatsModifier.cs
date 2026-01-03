using System;
using UnityEngine;

[Serializable]
public struct StatsModifier
{
    [Header("Flat Bonuses")]
    public int hpFlat;
    public int manaFlat;
    public int offenseFlat;
    public int defenseFlat;
    public int speedFlat;

    [Header("Percent Bonuses")] // 0.2 = +20%
    [Range(-1f, 3f)]
    public float hpPercent;
    [Range(-1f, 3f)]
    public float manaPercent;
    [Range(-1f, 3f)]
    public float offensePercent;  
    [Range(-1f, 3f)]
    public float defensePercent;
    [Range(-1f, 3f)]
    public float speedPercent;
}