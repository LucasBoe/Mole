using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITemporarySpawner : MonoBehaviour
{
    [SerializeField] TemporaryUIElement[] temporaryUIElements;
    Dictionary<Type, TemporaryUIElement> prefabDictionary;

    private void Awake()
    {
        prefabDictionary = new Dictionary<Type, TemporaryUIElement>();
        foreach (TemporaryUIElement element in temporaryUIElements)
            prefabDictionary.Add(element.GetType(), element);
    }

    public TemporaryUIElement Spawn<T>()
    {
        return Instantiate(prefabDictionary[typeof(T)], transform);
    }
    public TemporaryUIElement Spawn<T>(Transform parent)
    {
        return Instantiate(prefabDictionary[typeof(T)], parent);
    }
}
