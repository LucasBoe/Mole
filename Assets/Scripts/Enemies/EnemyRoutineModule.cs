using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoutineModule : EnemyModule<EnemyRoutineModule>
{
    public List<EnemyAIRoutineState> EnemyAIStates = new List<EnemyAIRoutineState>();

    EnemyMoveModule moveModule;

    private void OnEnable()
    {
        moveModule = GetComponent<EnemyMoveModule>();

        foreach (EnemyAIRoutineState state in EnemyAIStates)
        {
            if (state.Type == RoutineStateType.GoTo)
            {
                state.WorldPos = transform.TransformPoint(state.Pos);
            }
        }
    }

    public EnemyStateBase[] GetRoutineStates()
    {
        return EnemyAIStates.ToArray();
    }

    public bool Look(Vector2 vector2)
    {
        transform.localScale = new Vector3(vector2.x, transform.localScale.y, transform.localScale.z);
        return true;
    }

    private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
            Vector2 fromPos = transform.position;
            foreach (EnemyAIRoutineState state in EnemyAIStates)
            {
                if (state.Type == RoutineStateType.GoTo)
                {
                    Vector2 toPos = transform.TransformPoint(state.Pos);
                    Util.GizmoDrawArrowLine(fromPos, toPos);
                    fromPos = toPos;
                }
                else if (state.Type == RoutineStateType.Wait)
                {
                    Gizmos.DrawWireCube(fromPos, Vector3.one * 0.4f);
                }
            }
        }
    }