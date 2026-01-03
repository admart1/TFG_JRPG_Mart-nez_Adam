 using UnityEngine;

[CreateAssetMenu(menuName = "Sword/New Sword", fileName = "NewSword")]
public class EquipableSword : ScriptableObject
{
    [Header("Identidad")]
    public string swordId;
    public string displayName;

    [Header("Arte")]
    public Texture2D icon;

    [Header("Stats")]
    public StatsModifier statsModifier;

    [Header("Tipo")]
    public SwordType swordType = SwordType.Standard;
}