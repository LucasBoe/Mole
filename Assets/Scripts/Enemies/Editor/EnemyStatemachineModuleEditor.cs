using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyStatemachineModule))]
public class EnemyStatemachineModuleEditor : Editor
{
    EnemyStatemachineModule target;

    void OnEnable()
    {
        target = (EnemyStatemachineModule)serializedObject.targetObject;
    }

    override public void OnInspectorGUI()
    {
        if (target.CurrentState == null)
            return;

        GUIContent content = new GUIContent(StateToString(target.CurrentState));
        EditorGUILayout.HelpBox(content, wide: true);

        int i = 0;

        foreach (EnemyStateBase state in target.NextStates)
        {
            i++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i + ". ");
            EditorGUILayout.HelpBox(new GUIContent(StateToString(state)), wide: true);
            EditorGUILayout.EndHorizontal();
        }

        base.DrawDefaultInspector();
    }

    private string StateToString(EnemyStateBase state)
    {
        System.Type type = state.GetType();
        return type == typeof(EnemyAIRoutineState) ? "Routine ("+ (state as EnemyAIRoutineState).Type.ToString()+ ")" : type.ToString();
    }
}