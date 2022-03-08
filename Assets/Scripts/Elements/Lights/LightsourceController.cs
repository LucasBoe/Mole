using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightsourceController : AboveInputActionProvider
{
    [SerializeField] private ControllableLightsource lightsource;
    [SerializeField] private PlayerItem requiredItem;
    protected override void OnPlayerEnter()
    {
        inputActions[0].Text = lightsource.IsOn ? "Turn Off Light" : "Turn on Light";
        base.OnPlayerEnter();
    }

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction() { ActionCallback = TryInteract, Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Target = transform } };
    }

    public void TryInteract()
    {
        Log("TryInteract (on: " + lightsource.IsOn + ")");

        if (requiredItem != null)
        {
            if (PlayerItemHolder.Instance.GetAmount(requiredItem) == 0)
            {
                WorldTextSpawner.Spawn("0/1 " + requiredItem.name, transform.position);
                return;
            }
            else
            {
                PlayerItemHolder.Instance.RemoveItem(requiredItem);
            }
        }

        lightsource.Switch();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, lightsource.transform.position);
    }
}
