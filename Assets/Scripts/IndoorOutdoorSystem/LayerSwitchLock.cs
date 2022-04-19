using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add item based unlocking for switches
[RequireComponent(typeof(LayerSwitch))]
public class LayerSwitchLock : MonoBehaviour
{
    public bool IsLocked;
    private LayerSwitch layerSwitch;
    private void OnValidate()
    {
        LayerSwitch layerSwitch = GetComponent<LayerSwitch>();

        if (layerSwitch == null) return;

        layerSwitch.Lock = this;
        this.layerSwitch = layerSwitch;
    }


    internal bool TryUnlock()
    {

        if (!IsLocked) return true;

        WorldTextSpawner.Spawn("This door is locked...", transform.position);

        return false;
    }
}
