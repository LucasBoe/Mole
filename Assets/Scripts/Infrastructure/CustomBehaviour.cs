using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomBehaviour : MonoBehaviour
{
    protected void Log(string message)
    {
        Debug.Log(message);
    }

    protected void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    protected void LogError(string message)
    {
        Debug.LogError(message);
    }

    protected void Watch(string name, string value)
    {
        ConsoleProDebug.Watch(name, value);
    }
}
