using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : PlayerAboveInteractable
{
    [SerializeField] TriggerDoorPair left, right;
    private Dictionary<DoorTrigger, Layers> doorData = new Dictionary<DoorTrigger, Layers>();

    private void Start()
    {
        doorData.Add(left.Trigger, left.Layer);
        doorData.Add(right.Trigger, right.Layer);
    }

    internal void PlayerEntered(DoorTrigger doorTrigger)
    {
        if (DidNotJustSpawn())
        {
            Layers targetLayer = doorData[doorTrigger];
            LayerHandler.Instance.SetLayer(targetLayer);
        }
    }
}

[System.Serializable]
public class TriggerDoorPair
{
    public DoorTrigger Trigger;
    public Layers Layer;
}
