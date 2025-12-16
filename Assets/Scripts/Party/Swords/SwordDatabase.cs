using UnityEngine;

[CreateAssetMenu(menuName = "Swords/Sword Database", fileName = "NewSwordDatabase")]
public class SwordDatabase : ScriptableObject
{
    [Tooltip("todas las espadas del juego (SO).")]
    public EquipableSword[] allSwords;

    public EquipableSword GetSwordById(string swordId)
    {
        if (string.IsNullOrEmpty(swordId) || allSwords == null) return null;
        for (int i = 0; i < allSwords.Length; i++)
        {
            if (allSwords[i] != null && allSwords[i].swordId == swordId)
                return allSwords[i];
        }
        return null;
    }
}