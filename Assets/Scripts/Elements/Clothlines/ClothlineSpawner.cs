using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothlineSpawner : SingletonBehaviour<ClothlineSpawner>
{
    [SerializeField] Clothline clothlinePrefab;

    internal void Spawn(Vector3 start, Vector3 end)
    {
        Clothline clothline = Instantiate(clothlinePrefab, LayerHandler.Parent);
        clothline.StartTransform.position = start;
        clothline.EndTransform.position = end;
    }
}
