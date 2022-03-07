using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSingletonBehaviour<T> : PlayerBehaviour where T : PlayerSingletonBehaviour<T>
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
}
