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

    protected void Debug(string message)
    {
        switch (enemyBase.DebugMode)
        {
            case DebugModes.Info:
                UnityEngine.Debug.Log(message);
                break;

            case DebugModes.Warning:
                UnityEngine.Debug.LogWarning(message);
                break;

            case DebugModes.Error:
                UnityEngine.Debug.LogError(message);
                break;
        }
    }

    public void SendDebug(string message)
    {
        Debug(message);
    }
}
