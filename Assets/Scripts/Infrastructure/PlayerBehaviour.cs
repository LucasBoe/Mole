using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    protected void Log(string message)
    {
        Debug.Log("P:" + message);
    }

    protected void LogWarning(string message)
    {
        Debug.LogWarning("P:" + message);
    }

    protected void LogError(string message)
    {
        Debug.LogError("P:" + message);
    }

    protected void Watch(string name, string value )
    {
        ConsoleProDebug.Watch(name, value);
    }
}
