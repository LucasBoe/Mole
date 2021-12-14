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

    public EnemyAlertState(System.Action onTargetReached)
    {
        OnTargetReached = onTargetReached;
    }

    public override bool TryEnter(EnemyBase enemyBase)
    {
        memoryModule = enemyBase.GetModule<EnemyMemoryModule>();
        moveModule = enemyBase.GetModule<EnemyMoveModule>();

        if (memoryModule.TargetIsTransform)
            moveModule.FollowTransform(memoryModule.TargetTransform, OnTargetReached);
        else
            moveModule.MoveTo(memoryModule.TargetPos, OnTargetReached);

        return true;
    }
}

[System.Serializable]
public class EnemyLookAroundState : EnemyStateBase
{
    EnemyMemoryModule memoryModule;
    EnemyViewconeModule viewconeModule;

    bool doneLookingAround = false;

    public override bool TryEnter(EnemyBase enemyBase)
    {
        memoryModule = enemyBase.GetModule <EnemyMemoryModule>();
        viewconeModule = enemyBase.GetModule<EnemyViewconeModule>();

        Vector2 target = memoryModule.TargetIsTransform ? (Vector2)memoryModule.TargetTransform.position : memoryModule.TargetPos;
        float xDir = Mathf.Sign(target.x - enemyBase.transform.position.x);
        enemyBase.transform.localScale = new Vector3(xDir, enemyBase.transform.localScale.y, enemyBase.transform.localScale.z);

        viewconeModule.LookTo(memoryModule.TargetPos, andBack: true, Callback);

        return true;
    }

    public void Callback()
    {
        doneLookingAround = true;
    }

    public override bool TryExit(EnemyBase enemyBase)
    {
        return doneLookingAround;
    }
}
