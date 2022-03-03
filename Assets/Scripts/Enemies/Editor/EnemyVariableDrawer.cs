using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnemyVariable))]
public class EnemyVariableDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float lineHeight = position.height / 2;
        float typeWidth = 80;

        // Calculate rects
        var typeRect = new Rect(position.x, position.y, typeWidth, lineHeight);
        var nameRect = new Rect(position.x + typeWidth, position.y, position.width - typeWidth, lineHeight);
        var valueRect = new Rect(position.x, position.y + lineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, lineHeight);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("Type"), GUIContent.none);
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"), GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("localPosition"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        float totalHeight = (EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing) * 2;
        return totalHeight;
    }
}
