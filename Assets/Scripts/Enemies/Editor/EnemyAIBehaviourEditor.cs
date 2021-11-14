using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyAIRoutineModule))]
public class EnemyAIRoutineBehaviourEditor : Editor
{
    bool addMode = false;
    AIStateType toAdd;

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
            toAdd = (AIStateType)EditorGUILayout.EnumPopup("Tile Type", toAdd);
            EnemyAIRoutineModule aiBehaviour = (EnemyAIRoutineModule)target;
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