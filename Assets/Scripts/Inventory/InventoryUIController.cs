using UnityEngine.UIElements;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public InventoryController inventoryController;
    public PartyManager partyManager;

    private VisualElement root;
    private VisualElement swordList;
    private Button slot1Button;
    private Button slot2Button;
    private bool menuActive = false;

    private int selectedSlot = -1;

    void Start()
    {

        inventoryController.AddSwordById("sw_gray");
        inventoryController.AddSwordById("sw_red");
        inventoryController.AddSwordById("sw_green");

        RefreshSwordList();
        root.style.display = DisplayStyle.None;
        swordList.style.display = DisplayStyle.None;

        RefreshSlotButtons();
    }

    void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        swordList = root.Q<VisualElement>("SwordList");
        slot1Button = root.Q<Button>("Slot1Button");
        slot2Button = root.Q<Button>("Slot2Button");

        slot1Button.clicked += () => OnSlotSelected(1);
        slot2Button.clicked += () => OnSlotSelected(2);
    }

    void OnSlotSelected(int slot)
    {
        selectedSlot = slot;
        swordList.style.display = DisplayStyle.Flex;
    }

    void RefreshSwordList()
    {
        swordList.Clear();

        foreach (var sword in inventoryController.GetOwnedSwords())
        {
            var button = new Button();
            button.text = sword.displayName;

            button.clicked += () => EquipSwordToSelectedSlot(sword);

            swordList.Add(button);
        }
    }

    void EquipSwordToSelectedSlot(EquipableSword sword)
    {
        if (selectedSlot == -1) return;

        partyManager.activeCharacter.EquipSword(sword, selectedSlot);

        selectedSlot = -1;

        RefreshSlotButtons();

        swordList.style.display = DisplayStyle.None;
    }

    void RefreshSlotButtons()
    {
        var activeCharacter = partyManager.activeCharacter;

        slot1Button.text = activeCharacter.SwordSlot1.displayName;
        slot2Button.text = activeCharacter.SwordSlot2.displayName;
    }

    public void ToggleMenu()
    {
        menuActive = !menuActive;
        root.style.display = menuActive ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
