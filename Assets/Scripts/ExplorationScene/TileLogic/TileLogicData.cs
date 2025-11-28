using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/Tile Logic Data")]
public class TileLogicData : ScriptableObject
{
    public TileLogicType type;

    [Header("Generic Flags")]
    public bool isWalkable = true;
    public bool isBlocking = false;
}