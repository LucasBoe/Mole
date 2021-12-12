using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateType
{
    Routine,
    Alert,
    Return,
    Attack,
    CheckEnvironment,
}

public class SimpleEnemy : EnemyBase
{
    [SerializeField] Vector2 eyePosition;
    [SerializeField] float viewConeDistance, viewConeHeight;

    //Modules
    EnemyRoutineModule routineModule;
    EnemyMoveModule moveModule;
    NoiseListener noiseListenerModule;
    [SerializeField] EnemyViewconeModule viewconeModule;

    [SerializeField] EnemyStateType Current;
    public System.Action<EnemyStateType> OnStateChange;

    public EnemyMemoryModule memory;

    private void Start()
    {
        memory.Callback = ReachedTarget;

        routineModule = GetComponent<EnemyRoutineModule>();
        moveModule = GetComponent<EnemyMoveModule>();
        noiseListenerModule = GetComponent<NoiseListener>();

        viewconeModule.OnPlayerEnter += AlertFollow;
        viewconeModule.OnPlayerExit += AlertStopFollow;
        noiseListenerModule.OnNoise += AlertMoveTo;

        SetState(EnemyStateType.Routine);
    }


    private void SetState(EnemyStateType behaviorState)
    {
        //stateDictionary[Current].Exit();
        //
        //Current = behaviorState;
        //OnStateChange?.Invoke(Current);
        //
        //stateDictionary[behaviorState].Enter();
    }

    public void AlertMoveTo(Vector2 pos)
    {
        memory.SetTarget(pos);
        memory.FollowupState = EnemyStateType.CheckEnvironment;
        SetState(EnemyStateType.Alert);
    }

    public void AlertFollow(Transform trans)
    {
        memory.SetTarget(trans);
        memory.FollowupState = EnemyStateType.CheckEnvironment;
        SetState(EnemyStateType.Alert);
    }
    private void AlertStopFollow(Vector2 lastFollowPos)
    {
        memory.SetTarget(lastFollowPos);
        SetState(EnemyStateType.CheckEnvironment);
    }

    public void ReachedTarget()
    {
        SetState(memory.FollowupState);
        memory.FollowupState = EnemyStateType.Routine;
    }

    internal void UpdateViewcone()
    {
        viewconeModule.UpdateBounds(eyePosition, viewConeDistance, viewConeHeight);
    }
}
