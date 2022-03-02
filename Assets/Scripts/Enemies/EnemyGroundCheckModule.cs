using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheckModule : EnemyModule<EnemyGroundCheckModule>
{
    [SerializeField, Layer]
    List<int> validLayers = new List<int>();
    [SerializeField, Layer, ReadOnly]
    List<int> currentLayers = new List<int>();

    public System.Action EnteredGround;
    public System.Action LeftGround;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (!currentLayers.Contains(layer) && validLayers.Contains(layer))
        {
            currentLayers.Add(layer);
            if (currentLayers.Count == 1)
                EnteredGround?.Invoke();
        }    
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int layer = collision.gameObject.layer;
        if (currentLayers.Contains(layer))
        {
            currentLayers.Remove(layer);
            if (currentLayers.Count == 0)
                this.Delay(0.01f, CheckForGround);
        }
    }

    private void CheckForGround()
    {
        if (currentLayers.Count == 0)
            LeftGround?.Invoke();
    }
}
