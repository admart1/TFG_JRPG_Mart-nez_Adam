using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyLayerController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Transform player;
    public PlayerController playerController;
    [SerializeField] public float yOffset = -0.65f;
    [SerializeField] public int objectHeight = 0;

    public string abovePlayerLayer = "AbovePlayer";
    public string belowPlayerLayer = "BelowPlayer";

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = FindAnyObjectByType<PlayerController>();
    }

    void LateUpdate()
    {
        if (playerController.heightSystem.currentHeight == objectHeight || playerController.heightSystem.isFalling)
        {

            float enemyY = transform.position.y + yOffset;
            float playerY = player.position.y;

            if (enemyY > playerY)
                spriteRenderer.sortingLayerName = belowPlayerLayer;
            else
                spriteRenderer.sortingLayerName = abovePlayerLayer;
        } else
        {
            spriteRenderer.sortingLayerName = belowPlayerLayer;
        }
    }
}