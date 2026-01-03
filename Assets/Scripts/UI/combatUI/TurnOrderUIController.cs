using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TurnOrderUIController : MonoBehaviour
{
    public TurnManager turnManager; 
    public VisualTreeAsset turnIconAsset;

    private VisualElement root;
    private VisualElement panel;

    private Dictionary<Combatant, VisualElement> iconMap = new();

    void Awake()
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        panel = root.Q<VisualElement>("TurnOrderPanel");

        turnManager.OnTurnStarted += RefreshTurnOrder;
    }

    void RefreshTurnOrder(Combatant current)
    {
        panel.Clear();
        iconMap.Clear();

        foreach (var combatant in turnManager.orderedCombatants)
        {
            VisualElement icon = turnIconAsset.CloneTree();
            var img = icon.Q<Image>("Avatar");

            if (combatant.CombatantType == CombatantType.Player)
                img.style.backgroundImage = combatant.PlayerSource.definition.playerIcon;
            else if (combatant.CombatantType == CombatantType.Enemy)
                img.style.backgroundImage = combatant.EnemySource.enemyIcon;

            if (combatant == current)
                icon.AddToClassList("turn-current");
            else
                icon.RemoveFromClassList("turn-current");

            panel.Add(icon);
            iconMap[combatant] = icon;
        }
    }
}
