using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIModule : EnemyModule<EnemyAIModule>
{
    private enum AIMode
    {
        Routine,
        Interrupted,
        Attack,
    }

    private AIMode mode;
    private AIMode Mode
    {
        get => mode;
        set => mode = value;
    }

    EnemyRoutineModule routineModule;
    EnemyStatemachineModule statemachineModule;
    EnemyViewconeModule viewconeModule;
    EnemyMemoryModule memoryModule;
    EnemyMoveModule moveModule;

    [SerializeField] NoiseListener noiseListener;


    private void Start()
    {
        statemachineModule = GetModule<EnemyStatemachineModule>();
        viewconeModule = GetModule<EnemyViewconeModule>();
        memoryModule = GetModule<EnemyMemoryModule>();
        routineModule = GetModule<EnemyRoutineModule>();
        moveModule = GetModule<EnemyMoveModule>();

        moveModule.OnStartMovingToPosition += SetViewconeModeToForward;
        viewconeModule.OnPlayerEnter += OnPlayerEnteredViewcone;
        //viewconeModule.OnPlayerExit += CheckOutLocation;
        noiseListener.OnNoise += CheckOutLocation;
    }

    private void SetViewconeModeToForward()
    {
        viewconeModule.SetViewconeMode(EnemyViewconeModule.ViewconeMode.LookForward);
    }

    private void CheckOutLocation(Vector2 location)
    {
        Mode = AIMode.Interrupted;
        memoryModule.SetTarget(location);

        Debug.LogWarning("check it out mode: " + Mode);

        statemachineModule.StopCurrent();
    }

    private void OnPlayerEnteredViewcone(Transform target)
    {
        Mode = AIMode.Attack;
        memoryModule.SetTarget(target);

        statemachineModule.StopCurrent();
    }

    public EnemyStateBase[] GetNextStates()
    {
        List<EnemyStateBase> newStates = new List<EnemyStateBase>();

        //newStates.Add(new EnemyWaitState(1));
        //
        //return newStates.ToArray();

        bool canSeeTarget = viewconeModule.CanSeeTarget;


        if (Mode == AIMode.Attack && !canSeeTarget)
        {
            SetViewconeModeToForward();
            memoryModule.SetTarget(viewconeModule.LastSeenTarget);
            Mode = AIMode.Interrupted;
        }
        else if (Mode == AIMode.Interrupted)
        {
            if (canSeeTarget)
                Mode = AIMode.Attack;
            else if (viewconeModule.LookedAroundCounter > 0)
                Mode = AIMode.Routine;
        }

        if (Mode == AIMode.Attack)
        {
            newStates.Add(new EnemyShootState(memoryModule.TargetTransform));
        }
        else if (Mode == AIMode.Interrupted)
        {
            newStates.Add(new EnemyWaitState(0.25f));
            newStates.Add(new EnemyAlertState());
            newStates.Add(new EnemyLookAroundState());
        }
        else
        {
            return routineModule.GetRoutineStates();
        }

        return newStates.ToArray();
    }
}
