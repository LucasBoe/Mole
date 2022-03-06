using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class EnemyBase : MonoBehaviour
{
    Dictionary<System.Type, object> modules = new Dictionary<System.Type, object>();
    public System.Action OnEnemyDeath;
    public DebugModes DebugMode;

    public void Register<T>(object module)
    {
        modules.Add(typeof(T), module);
    }

    public T GetModule<T>()
    {
        if (modules.ContainsKey(typeof(T)))
            return (T)modules[typeof(T)];

        Debug.LogWarning("tried getting module of type " + typeof(T).ToString());
        return default(T);
    }
}

public enum DebugModes
{
    None,
    Info,
    Warning,
    Error,
}
