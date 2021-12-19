using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITemporarySpawner : MonoBehaviour
{
    [SerializeField] PlayerActionProgressionVisualizerUI playerActionProgressionVisualizerPrefab;

    public TemporaryUIElement Spawn<T>()
    {
        return Instantiate(GetPrefabFromType<T>(), transform);
    }

    private TemporaryUIElement GetPrefabFromType<T>()
    {
        if (playerActionProgressionVisualizerPrefab.GetType() == typeof(T))
            return playerActionProgressionVisualizerPrefab;

        return null;
    }
}
