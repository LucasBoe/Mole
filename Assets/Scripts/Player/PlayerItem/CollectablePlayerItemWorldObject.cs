using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePlayerItemWorldObject : InteractablePlayerItemWorldObject
{
    public SpriteRenderer SpriteRenderer;

    private void OnEnable()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Collect()
    {
        Destroy(gameObject);
    }
}
