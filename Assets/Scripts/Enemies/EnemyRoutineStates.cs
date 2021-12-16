using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum LookDirection
{
    Left,
    Right
}
public enum RoutineStateType
{
    Wait,
    GoTo,
    Look,
}

[System.Serializable]
public class EnemyAIRoutineState : EnemyStateBase
{
    public RoutineStateType Type;
    public LookDirection Direction;
    public Vector2 Pos;
    public Vector2 WorldPos;
    public float Speed;
    public float Duration;
    private float waitTimer;

    EnemyViewconeModule viewconeModule;
    EnemyMoveModule moveModule;

    public override bool TryEnter(EnemyBase enemyBase)
    {
        viewconeModule = enemyBase.GetModule<EnemyViewconeModule>();

        waitTimer = 0f;
        if (Type == RoutineStateType.GoTo)
        {
            moveModule = enemyBase.GetModule<EnemyMoveModule>();
            moveModule.MoveTo(WorldPos);
        }
        else if (Type == RoutineStateType.Look)
            viewconeModule.Look(Direction);

        return true;
    }

    public override bool TryExit(EnemyBase enemyBase)
    {
        switch (Type)
        {
            case RoutineStateType.GoTo:
                return !moveModule.isMoving;

            case RoutineStateType.Look:
                return true;

            case RoutineStateType.Wait:
                return (waitTimer += Time.deltaTime) > Duration;
        }

        return false;
    }

    public override string ToString()
    {
        return "Routine: " + Type.ToString();
    }
}

[CustomPropertyDrawer(typeof(EnemyAIRoutineState))]
public class EnemyAIRoutineStateDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RoutineStateType type = RoutineStateType.Wait;

        bool draw = false;
        int index = 0;
        var childEnum = property.GetEnumerator();
        while (childEnum.MoveNext())
        {
            SerializedProperty current = childEnum.Current as SerializedProperty;
            if (current.name == "Type")
            {
                type = (RoutineStateType)current.enumValueIndex;
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

    private bool TypeContainsProperty(RoutineStateType type, string name)
    {
        switch (type)
        {
            case RoutineStateType.GoTo:
                if (name == "Pos" || name == "Speed" || name == "WorldPos")
                    return true;
                break;

            case RoutineStateType.Look:
                if (name == "Direction")
                    return true;
                break;

            case RoutineStateType.Wait:
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
