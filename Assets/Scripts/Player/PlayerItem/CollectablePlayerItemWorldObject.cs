using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePlayerItemWorldObject : InteractablePlayerItemWorldObject
{
    [SerializeField] GameObject customGameObject;

    public virtual void Collect()
    {
        if (customGameObject != null)
            Destroy(customGameObject);
        else
            Destroy(gameObject);
    }
}
