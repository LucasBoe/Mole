using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStateBase
{
    public virtual bool TryEnter(EnemyBase enemyBase)
    {
        return true;
    }

    public virtual void Update(EnemyBase enemyBase)
    {

    }
    public virtual bool TryExit(EnemyBase enemyBase)
    {
        return true;
    }

    public override string ToString()
    {
        return GetType().ToString();
    }

    public virtual void ForceExit()
    {

    }
}

[System.Serializable]
public class EnemyWaitState : EnemyStateBase
{
    private float startTime = 0;
    private float waitTime;

    public EnemyWaitState(float waitTime)
    {
        this.waitTime = waitTime;
    }

    public override bool TryEnter(EnemyBase enemyBase)
    {
        startTime = Time.time;
        return true;
    }

    public override bool TryExit(EnemyBase enemyBase)
    {
        return Time.time > startTime + waitTime;
    }
}

[System.Serializable]
public class EnemyAlertState : EnemyStateBase
{
    EnemyMoveModule moveModule;
    EnemyMemoryModule memoryModule;
    System.Action OnTargetReached;

    public override bool TryEnter(EnemyBase enemyBase)
    {
        memoryModule = enemyBase.GetModule<EnemyMemoryModule>();
        moveModule = enemyBase.GetModule<EnemyMoveModule>();

        if (memoryModule.TargetIsTransform)
            moveModule.FollowTransform(memoryModule.TargetTransform);
        else 
            moveModule.MoveTo(memoryModule.TargetPos);

        return true;
    }

    public override void ForceExit()
    {
        //moveModule.StopMoving();
    }

    public override bool TryExit(EnemyBase enemyBase)
    {
        return !moveModule.isMoving;
    }
}

[System.Serializable]
public class EnemyLookAroundState : EnemyStateBase
{
    EnemyMemoryModule memoryModule;
    EnemyViewconeModule viewconeModule;

    public override bool TryEnter(EnemyBase enemyBase)
    {
        memoryModule = enemyBase.GetModule <EnemyMemoryModule>();
        viewconeModule = enemyBase.GetModule<EnemyViewconeModule>();
        viewconeModule.SetViewconeMode(EnemyViewconeModule.ViewconeMode.ScanSurrounding);

        return true;
    }

    public override bool TryExit(EnemyBase enemyBase)
    {
        return viewconeModule.Done;
    }

    public override void ForceExit()
    {
        viewconeModule.SetViewconeMode(EnemyViewconeModule.ViewconeMode.Free);
    }
}
