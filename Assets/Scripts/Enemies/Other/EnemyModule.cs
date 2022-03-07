using System;
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

    protected void Log(string message)
    {
        switch (enemyBase.DebugMode)
        {
            case DebugModes.Info:
                Debug.Log(message);
                break;

            case DebugModes.Warning:
                Debug.LogWarning(message);
                break;

            case DebugModes.Error:
                Debug.LogError(message);
                break;
        }
    }

    private void Watch(string name, string message)
    {
        if (enemyBase.DebugMode != DebugModes.None)
            ConsoleProDebug.Watch(name, message);
    }

    public void SendLog(string message)
    {
        Log(message);
    }

    public void SendWatch(string name, string message)
    {
        Watch(name, message);
    }
}
