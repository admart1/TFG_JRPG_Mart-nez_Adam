using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DeathLogController : MonoBehaviour
{
    private Label logLabel;
    private List<string> entries = new();

    private int maxLines = 5;

    private void Awake()
    {
        var ui = GetComponent<UIDocument>();
        logLabel = ui.rootVisualElement.Q<Label>("DeathLogLabel");
    }

    public void Add(string entry)
    {
        entries.Add(entry);

        if (entries.Count > maxLines)
            entries.RemoveAt(0);

        logLabel.text = string.Join("\n", entries);
    }

    public void Clear()
    {
        entries.Clear();
        logLabel.text = "";
    }
}