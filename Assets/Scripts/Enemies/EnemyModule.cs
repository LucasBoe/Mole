using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModule<T> : MonoBehaviour
{
    protected EnemyBase enemyBase;
    protected virtual void Awake()
    {
        enemyBase = GetComponentInParent<EnemyBase>();
        enemyBase.Register<T>(this);
    }

    protected T1 GetModule<T1>()
    {
        return enemyBase.GetModule<T1>();
    }
}
