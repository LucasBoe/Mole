using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatemachineModule : EnemyModule<EnemyStatemachineModule>
{
    private LinkedList<EnemyStateBase> list = new LinkedList<EnemyStateBase>();
    private EnemyStateBase currentState;

    private void Update()
    {
        if (currentState == null)
        {
            EnemyStateBase newState = FetchNewStateFromTop();
            if (newState.TryEnter(enemyBase))
                currentState = newState;
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
            foreach (EnemyStateBase state in GetModule<EnemyRoutineModule>().GetRoutineStates())
                list.AddLast(state);
        }

        EnemyStateBase toReturn = list.First.Value;
        list.RemoveFirst();
        return toReturn;
    }

    public void OverrideState(EnemyStateBase state)
    {
        list.AddFirst(state);
        currentState = null;
    }
}
