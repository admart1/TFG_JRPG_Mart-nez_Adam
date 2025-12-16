using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUDController : MonoBehaviour
{
    public PartyManager partyManager;

    private VisualElement root;
    private ProgressBar healthBar;
    private ProgressBar manaBar;
    private Image activeSwordIcon;
    private Label activeSwordName;
    private CharacterModel character;


    void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        healthBar = root.Q<ProgressBar>("HealthBar");
        manaBar = root.Q<ProgressBar>("ManaBar");
        activeSwordIcon = root.Q<Image>("ActiveSwordIcon");
        activeSwordName = root.Q<Label>("ActiveSwordName");
    }

    void Update()
    {
        if (partyManager?.activeCharacter == null) return;

        character = partyManager.activeCharacter;

        healthBar.value = character.currentHP;
        healthBar.highValue = character.definition.baseStats.maxHP;

        manaBar.value = character.currentMana;
        manaBar.highValue = character.definition.baseStats.maxMana;

        var sword = character.GetActiveSword();
        if (sword != null)
        {
            activeSwordIcon.style.backgroundImage = sword.icon;
            activeSwordName.text = sword.displayName;
        }
        else
        {
            activeSwordIcon.style.backgroundImage = null;
            activeSwordName.text = "None";
        }
    }
}
