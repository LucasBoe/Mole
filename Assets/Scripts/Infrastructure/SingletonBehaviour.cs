using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBehaviour<T> : CustomBehaviour where T : SingletonBehaviour<T>
{
    public static T Instance => instance;
    private static T instance;

    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this as T;
    }
#if UNITY_EDITOR
    protected static void OverrideInstanceEditor(T i)
    {
        instance = i;
    }
#endif
}
