using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

//TODO: Add item based unlocking for switches
[RequireComponent(typeof(LayerSwitch))]
public class LayerSwitchLock : MonoBehaviour
{
    public bool IsLocked;
    private LayerSwitch layerSwitch;

    [SerializeField] bool HasKeyItem;
    [SerializeField, ShowIf("HasKeyItem")] PlayerItem keyItem;
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

        if (HasKeyItem)
        {
            if (PlayerItemHolder.Instance.GetAmount(keyItem) > 0)
            {
                PlayerItemHolder.Instance.RemoveItem(keyItem);
                IsLocked = false;
                WorldTextSpawner.Spawn("used " + keyItem.name, transform.position);
                return true;

            } else
            {
                WorldTextSpawner.Spawn("0 / 1 " + keyItem.name, transform.position);
                return false;
            }
        }

        WorldTextSpawner.Spawn("This door is locked...", transform.position);

        return false;
    }
}
