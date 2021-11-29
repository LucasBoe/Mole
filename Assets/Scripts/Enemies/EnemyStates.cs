using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState
{
    public virtual void Enter() { }
    public virtual void Exit() { }
}

public class EnemyRoutineState : EnemyBaseState
{
    EnemyAIRoutineModule routineModule;
    public EnemyRoutineState (EnemyAIRoutineModule module)
    {
        routineModule = module;
    }

    public override void Enter()
    {
        routineModule.StartRoutine();
    }

    public override void Exit()
    {
        routineModule.StopRoutine();
    }
}

public class EnemyAlertState : EnemyBaseState
{
    EnemyAIMoveModule moveModule;
    EnemyMemory memoryModule;
    SimpleEnemy stateMachine;

    public EnemyAlertState(EnemyAIMoveModule module, EnemyMemory memory, SimpleEnemy stateMachine)
    {
        moveModule = module;
        memoryModule = memory;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        moveModule.MoveTo(memoryModule.Target, memoryModule.Callback);
    }
}
