using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyMemoryModule : EnemyModule<EnemyMemoryModule>
{
    public EnemyMemoryTargetType TargetType;
    public Transform TargetTransform;
    public Vector2 TargetPos;
    public bool TargetIsTransform => TargetType == EnemyMemoryTargetType.Tranform;
    public System.Action Callback;
    public EnemyStateType FollowupState;


    public void SetTarget(Transform target)
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
