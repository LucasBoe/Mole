using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIRoutineModule : MonoBehaviour
{
    public List<EnemyAIRoutineState> EnemyAIStates = new List<EnemyAIRoutineState>();

    private void OnEnable()
    {
        foreach (EnemyAIRoutineState state in EnemyAIStates)
        {
            if (state.Type == AIStateType.GoTo)
            {
                state.WorldPos = transform.TransformPoint(state.Pos);
            }
        }
    }

    public void StartRoutine()
    {
        StartCoroutine(ProcessRoutineAIRoutine());
    }

    public void StopRoutine()
    {
        StopAllCoroutines();
    }

    private IEnumerator ProcessRoutineAIRoutine()
    {
        int stateIndex = 0;
        while (true)
        {
            EnemyAIStates[stateIndex].Enter();
            while (!EnemyAIStates[stateIndex].Update(this))
            {
                yield return null;
            }
            stateIndex++;

            if (stateIndex >= EnemyAIStates.Count)
                stateIndex = 0;

            yield return null;
        }
    }

    public bool Look(Vector2 vector2)
    {
        transform.localScale = new Vector3(vector2.x, transform.localScale.y, transform.localScale.z);
        return true;
    }

    public bool MoveTowards(Vector2 worldPos, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, worldPos, speed * Time.deltaTime);
        return (Vector2.Distance(transform.position, worldPos) < 0.01f);
    }

    private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
            Vector2 fromPos = transform.position;
            foreach (EnemyAIRoutineState state in EnemyAIStates)
            {
                if (state.Type == AIStateType.GoTo)
                {
                    Vector2 toPos = transform.TransformPoint(state.Pos);
                    Util.GizmoDrawArrowLine(fromPos, toPos);
                    fromPos = toPos;
                }
                else if (state.Type == AIStateType.Wait)
                {
                    Gizmos.DrawWireCube(fromPos, Vector3.one * 0.4f);
                }
            }
        }
    }
