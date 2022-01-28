using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : PlayerAboveInteractable
{
    [SerializeField] Layers layerLeadsTo;
    protected override void OnPlayerEnter()
    {
        LayerHandler.Instance.SetLayer(layerLeadsTo);
    }
}
