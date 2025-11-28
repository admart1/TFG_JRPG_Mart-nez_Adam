using UnityEngine;
using UnityEngine.Tilemaps;

public class TileLogicDatabase : MonoBehaviour
{
    public static TileLogicDatabase Instance { get; private set; }

    [SerializeField] public Tilemap logicTilemap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[TileLogicDatabase] Multiple instances found. Destroying extra.");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public TileLogicData GetLogicAt(Vector3 worldPos)
    {
        if (logicTilemap == null)
        {
            Debug.LogWarning("[TileLogicDatabase] logicTilemap is null.");
            return null;
        }

        Vector3Int cell = logicTilemap.WorldToCell(worldPos);
        TileLogicAsset tile = logicTilemap.GetTile<TileLogicAsset>(cell);

        return tile == null ? null : tile.logic;
    }
}