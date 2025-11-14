using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DBDisplayName), true)]
public class DisplayNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var displayNameProp = property.FindPropertyRelative("animationName");
        if (displayNameProp != null && !string.IsNullOrEmpty(displayNameProp.stringValue))
        {
            label.text = displayNameProp.stringValue;
        }

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
