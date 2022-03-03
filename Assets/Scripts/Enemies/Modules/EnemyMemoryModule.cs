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

    public int ScannedCounter = 0;

    [SerializeField] GameObject lastSeenTargetEffectPrefab;

    public void SetTarget(Transform target)
    {
        TargetType = EnemyMemoryTargetType.Tranform;
        TargetPos = Vector2.zero;
        TargetTransform = target;
    }

    public void SetTarget(Vector2 target)
    {
        ScannedCounter = 0;
        TargetType = EnemyMemoryTargetType.Vector2;
        TargetPos = target;
        TargetTransform = null;

        
        if (target != Vector2.zero)
            EffectHandler.Spawn(new CustomEffect(lastSeenTargetEffectPrefab, 3f), target );
    }

    public enum EnemyMemoryTargetType
    {
        Tranform,
        Vector2,
    }
}
