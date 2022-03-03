using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnemyVariableReference))]
public class EnemyVariableReferenceDrawer : PropertyDrawer
{
    int _choiceIndex;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty userIndexProperty = property.FindPropertyRelative("VarName");

        EditorGUI.BeginChangeCheck();
        string[] variables = EnemyVariableModule.GetVariables();
        string selected = userIndexProperty.stringValue;

        int index = 0;
        for (int i = 0; i < variables.Length; i++)
        {
            if (selected == variables[i])
                index = i;
        }

        _choiceIndex = EditorGUI.Popup(position, index, variables);
        if (EditorGUI.EndChangeCheck())
        {
            userIndexProperty.stringValue = variables[_choiceIndex];
        }
    }
}
