using UnityEngine;

[CreateAssetMenu(menuName = "Sword/New Sword", fileName = "NewSword")]
public class EquipableSword : ScriptableObject
{
    [Header("Identidad")]
    public string swordId;
    public string displayName;

    [Header("Arte")]
    public Sprite icon;

    [Header("Stats")]
    public StatsModifier statsModifier;
}