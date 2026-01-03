using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Enemy Group")]
public class EnemyGroupData : ScriptableObject
{
    public List<EnemyData> enemies;
}

public enum EnemyWeakness
{
    None,
    Fire,
    Ice
}

public enum EnemyResistance
{
    None,
    Standard,
    Fire,
    Ice
}

[System.Serializable]
public class EnemyData
{
    public string enemyName;
    public string enemyID;
    public int maxHP;
    public int offense;
    public int defense;
    public int speed;
    public int level;
    
    public EnemyWeakness weakness = EnemyWeakness.None;
    public EnemyResistance resistance = EnemyResistance.None;

    public Sprite combatSprite;
    public Texture2D enemyIcon;
}
