using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyStatemachineModule : EnemyModule<EnemyStatemachineModule>
{
    private enum NewStateSource
    {
        RoutineModule,
        AIModule,
    }

    private LinkedList<EnemyStateBase> list = new LinkedList<EnemyStateBase>();
    private EnemyStateBase currentState;

    [SerializeField] private NewStateSource onEmptyNewStateSource;

    public System.Action<System.Type> OnEnterNewState;
    public EnemyStateBase CurrentState => currentState;
    public LinkedList<EnemyStateBase> NextStates => list;

    private void Update()
    {
        if (currentState == null)
        {
            EnemyStateBase newState = FetchNewStateFromTop();
            if (newState.TryEnter(enemyBase))
            {
                OnEnterNewState?.Invoke(newState.GetType());
                currentState = newState;
            }
        }
        else
        {
            currentState.Update(enemyBase);
            if (currentState.TryExit(enemyBase))
            {
                currentState = null;
            }
        }
    }

    private EnemyStateBase FetchNewStateFromTop()
    {
        if (list.Count == 0)
        {
            EnemyStateBase[] newStates = (onEmptyNewStateSource == NewStateSource.RoutineModule)
                ? GetModule<EnemyRoutineModule>().GetRoutineStates()
                : GetModule<EnemyAIModule>().GetNextStates();

            foreach (EnemyStateBase state in newStates)
                list.AddLast(state);
        }

        EnemyStateBase toReturn = list.First.Value;
        list.RemoveFirst();
        return toReturn;
    }

    public void OverrideState(EnemyStateBase state)
    {
        Debug.LogWarning("Override State: " + state.ToString());
        list.AddFirst(state);
        StopCurrent();
    }
    public void StopCurrent()
    {
        if (currentState != null)
        {
            currentState.ForceExit();
            currentState = null;
        }

        list.Clear();
    }

    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;

        if (currentState != null)
            Handles.Label(transform.position + Vector3.up, currentState.ToString(), style);
    }

}
