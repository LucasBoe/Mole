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
    public EnemyRoutineState(EnemyAIRoutineModule module)
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
        if (memoryModule.TargetIsTransform)
            moveModule.FollowTransform(memoryModule.TargetTransform, memoryModule.Callback);
        else
            moveModule.MoveTo(memoryModule.TargetPos, memoryModule.Callback);
    }
}
public class EnemyLookAroundState : EnemyBaseState
{
    EnemyMemory memoryModule;
    EnemyViewcone viewconeModule;
    SimpleEnemy stateMachine;

    public EnemyLookAroundState(EnemyMemory memory, EnemyViewcone viewcone, SimpleEnemy stateMachine)
    {
        memoryModule = memory;
        viewconeModule = viewcone;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        Vector2 target = memoryModule.TargetIsTransform ? (Vector2)memoryModule.TargetTransform.position : memoryModule.TargetPos;
        float xDir = Mathf.Sign(target.x - stateMachine.transform.position.x);
        stateMachine.transform.localScale = new Vector3(xDir, stateMachine.transform.localScale.y, stateMachine.transform.localScale.z);

        viewconeModule.LookTo(memoryModule.TargetPos, andBack: true, Callback);
    }

    public void Callback()
    {
        stateMachine.ReachedTarget();
    }
}
