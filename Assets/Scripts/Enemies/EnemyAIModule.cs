using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInterfaceable
{
    bool IsNull { get; }
}


public interface ICombatTarget : IInterfaceable
{
    Vector2 StranglePosition { get; }
    void Kill();
    bool StartStrangling();
    void StopStrangling(Vector2 playerPos);
}

public class EnemyAIModule : EnemyModule<EnemyAIModule>, ICombatTarget
{
    private enum AIMode
    {
        Routine,
        Interrupted,
        Attack,
        BeeingStrangled,
    }

    private AIMode mode;
    private AIMode Mode
    {
        get => mode;
        set => mode = value;
    }
    public Vector2 StranglePosition => transform.position + Vector3.left;
    public bool IsNull => this == null;
    public Action OnStartBeeingStrangled;


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
        if (Mode == AIMode.BeeingStrangled)
            return;

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
        else if (Mode == AIMode.BeeingStrangled)
        {
            newStates.Add(new EnemyWaitState(CombatStrangleState.strangleDuration));
        }
        else
        {
            return routineModule.GetRoutineStates();
        }

        return newStates.ToArray();
    }

    public void Kill()
    {
        if (gameObject != null)
            Destroy(gameObject);
    }

    public bool StartStrangling()
    {
        if (Mode == AIMode.Attack)
            return false;

        Mode = AIMode.BeeingStrangled;
        moveModule.StopMoving();
        viewconeModule.IsPassive = true;
        statemachineModule.StopCurrent();
        OnStartBeeingStrangled?.Invoke();
        return true;

    }

    public void StopStrangling(Vector2 playerPos)
    {
        Mode = AIMode.Interrupted;
        viewconeModule.IsPassive = false;
        memoryModule.SetTarget(playerPos);

        statemachineModule.StopCurrent();
    }
}
