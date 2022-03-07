using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnemyVariableReference))]
public class EnemyVariableReferenceDrawer : PropertyDrawer
{
    int _choiceIndex;
    bool editMode = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty userIndexProperty = property.FindPropertyRelative("VarName");

        float EditButtonWidth = 50f;
        var varRect = new Rect(position.x, position.y, position.width - EditButtonWidth, position.height);
        var editRect = new Rect(position.width - EditButtonWidth, position.y, EditButtonWidth, position.height);

        string selected = userIndexProperty.stringValue;

        if (editMode)
        {
            EditorGUI.BeginChangeCheck();
            string[] variables = EnemyVariableModule.GetVariables();

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
                editMode = false;
            }
        } else
        {
            GUI.Box(varRect, "var : " + (selected == "" ? "<UNDEFINED>" : selected));
            if (GUI.Button(editRect, "Edit"))
            {
                editMode = true;
            }
        }
    }
}
