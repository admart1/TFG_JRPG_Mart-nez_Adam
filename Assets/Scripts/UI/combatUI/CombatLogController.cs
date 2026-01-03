using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Text;

public class CombatLogController : MonoBehaviour
{
    private Label logLabel;
    private StringBuilder logBuilder = new();
    private const int MAX_LINES = 3;

    private Queue<string> lines = new();

    // colores
    private static readonly Color DAMAGE_NORMAL = Color.white;
    private static readonly Color DAMAGE_CRIT = new Color(1f, 0.8f, 0.2f);   
    private static readonly Color DAMAGE_WEAK = new Color(1f, 0.4f, 0.4f);    
    private static readonly Color DAMAGE_RESIST = new Color(0.5f, 0.7f, 1f);   

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        logLabel = root.Q<Label>("CombatLogLabel");
    }

    public void Add(string message)
    {
        if (logLabel == null) return;

        lines.Enqueue(message);

        if (lines.Count > MAX_LINES)
            lines.Dequeue();

        logBuilder.Clear();
        foreach (var line in lines)
            logBuilder.AppendLine(line);

        logLabel.text = logBuilder.ToString();
    }

    public void Clear()
    {
        lines.Clear();
        logLabel.text = "";
    }

    public void LogAttack(string attacker, string target, DamageResult result)
    {
        string text = $"{attacker} ataca a {target} ";

        string damageText = Mathf.RoundToInt(result.damage).ToString();
        if (result.isCrit)
            damageText = $"<color=yellow>{damageText} </color>";
        else if (result.isWeak)
            damageText = $"<color=red>{damageText} </color>";
        else if (result.isResist)
            damageText = $"<color=blue>{damageText} </color>";

        Add($"{text} Daño: {damageText}");
    }
}
