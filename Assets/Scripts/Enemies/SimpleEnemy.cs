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

        viewconeModule.OnPlayerEntered += AlertMoveTo;
        noiseListenerModule.OnNoise += AlertMoveTo;

        stateDictionary.Add(EnemyStateType.Routine, new EnemyRoutineState(routineModule));
        stateDictionary.Add(EnemyStateType.Alert, new EnemyAlertState(moveModule, memory, this));

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
        memory.Target = pos;
        SetState(EnemyStateType.Alert);
    }

    public void ReachedTarget ()
    {
        SetState(EnemyStateType.Routine);
    }

    internal void UpdateViewcone()
    {
        viewconeModule.UpdateBounds(eyePosition, viewConeDistance, viewConeHeight);
    }
}

public class EnemyMemory
{
    public Vector2 Target;
    public System.Action Callback;
}
