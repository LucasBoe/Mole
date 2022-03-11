using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePlayerItemWorldObject : InteractablePlayerItemWorldObject
{
    [SerializeField] GameObject customGameObject;

    public virtual bool Collect()
    {
        GameObject go = customGameObject != null ? customGameObject : gameObject;
        if (go != null)
        {
            Destroy(go);
            return true;
        }
        return false;
    }
}
