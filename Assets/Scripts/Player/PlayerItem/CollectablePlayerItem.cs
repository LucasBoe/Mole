using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePlayerItem : MonoBehaviour
{
    public PlayerItem Item;
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
