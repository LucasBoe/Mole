using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyStatemachineModule : EnemyModule<EnemyStatemachineModule>
{
    private LinkedList<EnemyStateBase> list = new LinkedList<EnemyStateBase>();
    private EnemyStateBase currentState;

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
                Debug.LogWarning("Entered: " + newState.ToString());
                OnEnterNewState?.Invoke(newState.GetType());
                currentState = newState;
            }
        }
        else
        {
            currentState.Update(enemyBase);
            if (currentState.TryExit(enemyBase))
                currentState = null;
        }
    }

    private EnemyStateBase FetchNewStateFromTop()
    {
        if (list.Count == 0)
        {
            Debug.LogWarning("States empty, fetch from routine");

            foreach (EnemyStateBase state in GetModule<EnemyRoutineModule>().GetRoutineStates())
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
        currentState = null;
    }

    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;

        if (currentState != null)
            Handles.Label(transform.position + Vector3.up, currentState.ToString(), style);
    }
}
