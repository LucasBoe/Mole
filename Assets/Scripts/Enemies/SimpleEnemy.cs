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

public class SimpleEnemy : MonoBehaviour
{
    [SerializeField] Vector2 eyePosition;
    [SerializeField] float viewConeDistance, viewConeHeight;

    //Modules
    EnemyAIRoutineModule routineModule;
    EnemyAIMoveModule moveModule;
    NoiseListener noiseListenerModule;
    [SerializeField] EnemyViewcone viewconeModule;

    [SerializeField] EnemyStateType Current;
    EnemyBaseState CurrentState;
    public System.Action<EnemyStateType> OnStateChange;

    private Dictionary<EnemyStateType, EnemyBaseState> stateDictionary = new Dictionary<EnemyStateType, EnemyBaseState>();

    public EnemyMemory memory;

    private void Start()
    {
        memory = new EnemyMemory();
        memory.Callback = ReachedTarget;

        routineModule = GetComponent<EnemyAIRoutineModule>();
        moveModule = GetComponent<EnemyAIMoveModule>();
        noiseListenerModule = GetComponent<NoiseListener>();

        viewconeModule.OnPlayerEnter += AlertFollow;
        viewconeModule.OnPlayerExit += AlertStopFollow;
        noiseListenerModule.OnNoise += AlertMoveTo;

        stateDictionary.Add(EnemyStateType.Routine, new EnemyRoutineState(routineModule));
        stateDictionary.Add(EnemyStateType.Alert, new EnemyAlertState(moveModule, memory, this));
        stateDictionary.Add(EnemyStateType.CheckEnvironment, new EnemyLookAroundState(memory, viewconeModule, this));

        SetState(EnemyStateType.Routine);
    }


    private void SetState(EnemyStateType behaviorState)
    {
        stateDictionary[Current].Exit();

        Current = behaviorState;
        OnStateChange?.Invoke(Current);

        stateDictionary[behaviorState].Enter();
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

public class EnemyMemory
{
    public EnemyMemoryTargetType TargetType;
    public Transform TargetTransform;
    public Vector2 TargetPos;
    public bool TargetIsTransform => TargetType == EnemyMemoryTargetType.Tranform;
    public System.Action Callback;
    public EnemyStateType FollowupState;


    public void SetTarget (Transform target)
    {
        TargetType = EnemyMemoryTargetType.Tranform;
        TargetTransform = target;
    }

    public void SetTarget(Vector2 target)
    {
        TargetType = EnemyMemoryTargetType.Vector2;
        TargetPos = target;
    }

    public enum EnemyMemoryTargetType
    {
        Tranform,
        Vector2,
    }
}
