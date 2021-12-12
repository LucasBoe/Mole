using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyRoutineModule))]
public class EnemyAIRoutineBehaviourEditor : Editor
{
    bool addMode = false;
    RoutineStateType toAdd;

    SerializedProperty list;

    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!addMode)
        {
            if (GUILayout.Button("Add New State"))
            {
                addMode = true;
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            toAdd = (RoutineStateType)EditorGUILayout.EnumPopup("Tile Type", toAdd);
            EnemyRoutineModule aiBehaviour = (EnemyRoutineModule)target;
            if (GUILayout.Button("Add"))
            {
                aiBehaviour.EnemyAIStates.Add(new EnemyAIRoutineState());
                addMode = false;
            }
            if (GUILayout.Button("Abort"))
            {
                addMode = false;
            }
            GUILayout.EndHorizontal();
        }
    }
}