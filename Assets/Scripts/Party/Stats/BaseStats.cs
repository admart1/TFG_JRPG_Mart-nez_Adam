using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Base Stats", fileName = "NewBaseStats")]
public class BaseStats : ScriptableObject
{
    [Header("Base Values")]
    public int maxHP = 10;
    public int offense = 1;
    public int defense = 1;
    public int speed = 1;

    public FinalStats ToStats()
    {
        return new FinalStats(maxHP, offense, defense, speed);
    }
}