using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum AIStateType
{
    Wait,
    GoTo,
    Look,
}

[System.Serializable]
public class EnemyAIRoutineState
{
    public AIStateType Type;
    public bool Right;
    public Vector2 Pos;
    public Vector2 WorldPos;
    public float Speed;
    public float Duration;
    private float waitTimer;
    public void Enter()
    {
        waitTimer = 0f;
    }

    public bool Update(EnemyAIRoutineModule enemyAIBehaviour)
    {
        switch (Type)
        {
            case AIStateType.GoTo:
                return enemyAIBehaviour.MoveTowards(WorldPos, Speed);

            case AIStateType.Look:
                return enemyAIBehaviour.Look(Right ? Vector2.right : Vector2.left);

            case AIStateType.Wait:
                return (waitTimer += Time.deltaTime) > Duration;
        }

        return false;
    }
}

[CustomPropertyDrawer(typeof(EnemyAIRoutineState))]
public class EnemyAIRoutineStateDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AIStateType type = AIStateType.Wait;

        bool draw = false;
        int index = 0;
        var childEnum = property.GetEnumerator();
        while (childEnum.MoveNext())
        {
            SerializedProperty current = childEnum.Current as SerializedProperty;
            if (current.name == "Type")
            {
                type = (AIStateType)current.enumValueIndex;
                draw = true;
            }
            else if (TypeContainsProperty(type, current.name))
            {
                draw = true;
            } else
            {
                draw = false;
            }

            if (draw)
            {
                Rect rect = new Rect(position.position + (Vector2.up * index * EditorGUIUtility.singleLineHeight), position.size);
                EditorGUI.PropertyField(rect, current, includeChildren: true);
                index++;
            }
        }
    }

    private bool TypeContainsProperty(AIStateType type, string name)
    {
        switch (type)
        {
            case AIStateType.GoTo:
                if (name == "Pos" || name == "Speed")
                    return true;
                break;

            case AIStateType.Look:
                if (name == "Right")
                    return true;
                break;

            case AIStateType.Wait:
                if (name == "Duration")
                    return true;
                break;
        }

        return false;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4f;
    }
}
