using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Level Progression", fileName = "NewLevelProgression")]
public class LevelProgression : ScriptableObject
{
    [Header("Modificadores por nivel")]
    public StatsModifier[] levelModifiers; 

    // devuelve el StatsModifier correspondiente al nivel actual
    public StatsModifier GetModifierForLevel(int level)
    {
        if (levelModifiers == null || levelModifiers.Length == 0)
            return new StatsModifier(); // sin modificaciones

        int index = Mathf.Clamp(level - 1, 0, levelModifiers.Length - 1);
        return levelModifiers[index];
    }
}