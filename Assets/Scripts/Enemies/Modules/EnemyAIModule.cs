using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIModule : EnemyModule<EnemyAIModule>, ICombatTarget
{
    private enum AIMode
    {
        Routine,
        Interrupted,
        Attack,
        BeeingStrangled,
        Falling,
    }

    [SerializeField, ReadOnly] private AIMode mode;
    private AIMode Mode
    {
        get => mode;
        set => mode = value;
    }
    public Vector2 StranglePosition => transform.position + Vector3.left;
    public bool IsNull => this == null;

    public System.Action OnStartBeeingStrangled;

    EnemyRoutineModule routineModule;
    EnemyStatemachineModule statemachineModule;
    EnemyViewconeModule viewconeModule;
    EnemyMemoryModule memoryModule;
    EnemyMoveModule moveModule;
    EnemyDamageModule damageModule;
    EnemyGroundCheckModule groundCheckModule;

    [SerializeField] private NoiseListener noiseListener;
    [SerializeField] private new Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D => rigidbody2D;

    [SerializeField] private EnemyColliderModule colliderModule;
    public ICollisionModifier CollisionModifier => colliderModule;

    private void Start()
    {
        statemachineModule = GetModule<EnemyStatemachineModule>();
        viewconeModule = GetModule<EnemyViewconeModule>();
        memoryModule = GetModule<EnemyMemoryModule>();
        routineModule = GetModule<EnemyRoutineModule>();
        moveModule = GetModule<EnemyMoveModule>();
        damageModule = GetModule<EnemyDamageModule>();
        groundCheckModule = GetModule<EnemyGroundCheckModule>();

        moveModule.OnStartMovingToPosition += SetViewconeModeToForward;
        viewconeModule.OnPlayerEnter += OnPlayerEnteredViewcone;
        noiseListener.OnNoise += CheckOutLocation;
        damageModule.OutOfHealth += Kill;
        groundCheckModule.EnteredGround += () => SetGrounded(true);
        groundCheckModule.LeftGround += () => SetGrounded(false);
    }

    private void SetGrounded(bool grounded)
    {

        if (Mode == AIMode.Falling && grounded)
        {
            statemachineModule.StopCurrent();
            Mode = AIMode.Interrupted;

            Debug.Log($"Mode = { Mode }");
        }
        else if (!grounded)
        {
            Mode = AIMode.Falling;

            Debug.Log($"Mode = { Mode }");
        }
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
        else if (Mode == AIMode.Falling)
        {
            newStates.Add(new EnemyWaitState(1f));
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
        {
            enemyBase.OnEnemyDeath?.Invoke();
            Destroy(gameObject);
        }

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
