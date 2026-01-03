using UnityEngine;

public class SwordPickup : MonoBehaviour
{
   private enum PickupState
    {
        Runtime,
        Paused
    }

    [Header("Sword")]
    [SerializeField] private string swordId;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private GameObject text;

    private PlayerController player;

    private bool playerInRange = false;
    private InventoryController inventory;
    private PickupState state = PickupState.Runtime;


    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        inventory = FindFirstObjectByType<InventoryController>();
    }

    void Update()
    {
        if (!playerInRange) return;

        switch (state)
        {
            case PickupState.Runtime:
                HandleRuntime();
                break;

            case PickupState.Paused:
                HandlePaused();
                break;
        }
    }

    private void HandleRuntime()
    {
        if (!player.input.AttackPressed) return;

        Time.timeScale = 0f;

        inventory.AddSwordById(swordId);
        player.inventoryUI.RefreshSwordList();
        text.SetActive(true);

        state = PickupState.Paused;
    }

    private void HandlePaused()
    {
        if (!player.input.AttackPressed) return;

        Time.timeScale = 1f;

        text.SetActive(false);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHurt")) return;

        playerInRange = true;

        if (interactIcon != null)
            interactIcon.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHurt")) return;

        playerInRange = false;

        if (interactIcon != null)
            interactIcon.SetActive(false);
    }
}
