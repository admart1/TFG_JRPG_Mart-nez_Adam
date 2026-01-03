using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System.Collections;
using static CombatManager;
using UnityEngine.EventSystems;

public class CombatUIController : MonoBehaviour
{
    [Header("Referencias")]
    public TurnManager turnManager;

    private VisualElement root;

    private class CharacterUI
    {
        public VisualElement root;
        public ProgressBar hpBar;
        public ProgressBar mpBar;
        public Label swordName;
        public Image swordIcon;
        public Label actionPointsLabel;
        public Label nameLabel;
    }

    private Dictionary<CharacterModel, CharacterUI> characterUIDict = new();
    private List<Combatant> allCombatants = new(); 
    private Combatant activeCombatant;
    private Combatant pendingActionTarget; 

    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        var actionMenu = root.Q<VisualElement>("ActionMenuContainer");
        if (actionMenu != null)
            actionMenu.style.display = DisplayStyle.None;

        turnManager.combatManager.OnCombatStarted += InitializePlayerUI;

        turnManager.OnTurnStarted += HandleTurnStarted;
        turnManager.OnTurnStateChanged += HandleTurnStateChanged;

        var playerParty = GameSession.Instance.PlayerParty;
        if (playerParty.Count > 0) SetupCharacterUI(playerParty[0], "CharacterOneInfo");
        if (playerParty.Count > 1) SetupCharacterUI(playerParty[1], "CharacterTwoInfo");

        var targetContainer = root.Q<VisualElement>("TargetListContainer");
        if (targetContainer != null)
            targetContainer.style.display = DisplayStyle.None;

        SetupActionButtons();
        SetupConfirmButton();
        SetupChangeSwordButton();
        SetupFleeButton();


        root.RegisterCallback<PointerDownEvent>(evt =>
        {
            Debug.Log("CLICK DETECTED EN ROOT");
        });
    }



    #region CHARACTER UI

    private void InitializePlayerUI()
    {
        foreach (var c in turnManager.combatManager.playerCombatants)
        {
            UpdateCharacterUI(c.PlayerSource);
            UpdateCharacterHPFromCombatant(c);
        }
    }

    private void SetupCharacterUI(CharacterModel model, string uxmlName)
    {
        var container = root.Q<VisualElement>(uxmlName);
        if (container == null) return;

        var ui = new CharacterUI
        {
            root = container,
            hpBar = container.Q<ProgressBar>("HealthBar"),
            mpBar = container.Q<ProgressBar>("ManaBar"),
            swordName = container.Q<Label>("ActiveSwordName"),
            swordIcon = container.Q<Image>("ActiveSwordIcon"),
            actionPointsLabel = container.Q<Label>("ActionPointsLabel"),
            nameLabel = container.Q<Label>("CharacterNameLabel")
        };

        characterUIDict[model] = ui;
        UpdateCharacterUI(model);
    }

    private void UpdateCharacterUI(CharacterModel model)
    {
        if (!characterUIDict.ContainsKey(model)) return;

        foreach (var c in turnManager.combatManager.playerCombatants)
        {
            UpdateCharacterHPFromCombatant(c);
        }

        var ui = characterUIDict[model];

        if (ui.nameLabel != null)
        {
            ui.nameLabel.text = model.definition.displayName;
        }

        ui.hpBar.highValue = model.GetFinalStats().maxHP;

        ui.mpBar.highValue = model.GetFinalStats().maxMana;
        ui.mpBar.value = model.currentMana;

        var activeSword = model.GetActiveSword();
        ui.swordName.text = activeSword != null ? activeSword.displayName : "None";
        if (ui.swordIcon != null && activeSword?.icon != null)
            ui.swordIcon.style.backgroundImage = activeSword.icon;

        if (activeCombatant != null && model == activeCombatant.PlayerSource)
            ui.actionPointsLabel.text =
                $"AP: {activeCombatant.CurrentActionPoints}/{activeCombatant.maxActionPoints}";
    }

    private void UpdateCharacterHPFromCombatant(Combatant combatant)
    {
        if (combatant == null) return;
        if (combatant.CombatantType != CombatantType.Player) return;

        var model = combatant.PlayerSource;
        if (model == null) return;
        if (!characterUIDict.ContainsKey(model)) return;

        var ui = characterUIDict[model];

        int maxHP = model.GetFinalStats().maxHP;

        ui.hpBar.highValue = maxHP;
        ui.hpBar.value = Mathf.Clamp(combatant.CurrentHP, 0, maxHP);
    }

    #endregion

    #region ACTION BUTTONS

    private void SetupActionButtons()
    {
        var attackButton = root.Q<Button>("AttackButton");

        attackButton.clicked += () =>
        {
            if (turnManager.CurrentTurnState != TurnState.WaitingForInput) return;
            ShowTargetSelection();
        };
    }

    private void SetupChangeSwordButton()
    {
        var button = root.Q<Button>("ChangeSwordButton");
        button.clicked += () =>
        {
            if (turnManager.CurrentTurnState != TurnState.WaitingForInput) return;

            var activeCombatant = turnManager.currentCombatant;
            if (activeCombatant == null || activeCombatant.CombatantType != CombatantType.Player) return;

            activeCombatant.PlayerSource.SwitchActiveSword();
            UpdateCharacterUI(activeCombatant.PlayerSource);

            var log = Object.FindFirstObjectByType<DeathLogController>();
            log?.Add($"{activeCombatant.GetDisplayName()} cambia a {activeCombatant.PlayerSource.GetActiveSword()?.displayName}");
        };
    }

    private void SetupConfirmButton()
    {
        var confirmButton = root.Q<Button>("ConfirmButton");
        confirmButton.clicked += () =>
        {
            if (turnManager.CurrentTurnState != TurnState.WaitingForInput) return;

            HideTargetSelection();

            activeCombatant.ExecutePlannedActions();
            turnManager.EndTurn();

            foreach (var c in turnManager.combatManager.playerCombatants)
            {
                UpdateCharacterHPFromCombatant(c);
            }
        };
    }

    void SetupFleeButton()
    {

        var fleeButton = root.Q<Button>("FleeButton");
        fleeButton.clicked += () =>
        {
            if (turnManager.CurrentTurnState != TurnState.WaitingForInput) return;

            turnManager.combatManager.OnCombatEnd(CombatResult.Escape);
        };
    }

    private void HideTargetSelection()
    {
        var targetContainer = root.Q<VisualElement>("TargetListContainer");
        if (targetContainer == null) return;

        targetContainer.style.display = DisplayStyle.None;
        targetContainer.Clear();
    }

    private void ShowTargetSelection()
    {
        var targetContainer = root.Q<VisualElement>("TargetListContainer");
        if (targetContainer == null) return;

        targetContainer.Clear();

        allCombatants = new List<Combatant>();
        allCombatants.AddRange(turnManager.combatManager.playerCombatants.Where(c => c.IsAlive));
        allCombatants.AddRange(turnManager.combatManager.enemyCombatants.Where(c => c.IsAlive));

        foreach (var target in allCombatants)
        {
            string displayName = target.CombatantType == CombatantType.Player
                ? target.GetDisplayName()
                : target.GetDisplayName();

            var button = new Button(() =>
            {
                activeCombatant.AddAction(new AttackAction(activeCombatant, target));

                UpdateCharacterUI(activeCombatant.PlayerSource);

                targetContainer.style.display = DisplayStyle.None;
                targetContainer.Clear(); 
            })
            {
                text = $"{displayName}"
            };

            targetContainer.Add(button);
        }

        targetContainer.style.display = DisplayStyle.Flex;
    }

    private void ClearTurnHighlight()
    {
        foreach (var ui in characterUIDict.Values)
        {
            ui.root.RemoveFromClassList("active-turn");
        }
    }

    


    #endregion

    #region TURN MANAGER EVENTS

    private void HandleTurnStarted(Combatant c)
    {
        activeCombatant = c;

        ClearTurnHighlight();

        if (c.CombatantType == CombatantType.Player &&
    characterUIDict.TryGetValue(c.PlayerSource, out var ui))
        {
            ui.root.AddToClassList("active-turn");
        }

        if (c.CombatantType == CombatantType.Player)
        {
            UpdateCharacterUI(c.PlayerSource);
        }
    }

    private void HandleTurnStateChanged(TurnState state)
    {
        root.Q<VisualElement>("ActionMenuContainer").style.display =
            state == TurnState.WaitingForInput ? DisplayStyle.Flex : DisplayStyle.None;
    }

    #endregion
}
