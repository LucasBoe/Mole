using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightsourceController : AboveInputActionProvider
{
    [SerializeField] private List<ControllableLightsource> lightSources;
    [SerializeField] private PlayerItem requiredItem;
    protected override void OnPlayerEnter()
    {
        inputActions[0].Text = lightSources[0].IsOn ? "Turn Off Light" : "Turn on Light";
        base.OnPlayerEnter();
    }

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction() { ActionCallback = TryInteract, Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Target = transform } };
    }

    public void TryInteract()
    {
        if (requiredItem != null)
        {
            if (PlayerItemHolder.Instance.GetAmount(requiredItem) == 0)
            {
                WorldTextSpawner.Spawn(requiredItem.Sprite, "0/1 " + requiredItem.name, transform.position);
                return;
            }
            else
            {
                PlayerItemHolder.Instance.RemoveItem(requiredItem);
            }
        }

        foreach (ControllableLightsource lightsource in lightSources)
        {
        lightsource.Switch();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (ControllableLightsource lightsource in lightSources)
        {
            Gizmos.DrawLine(transform.position, lightsource.transform.position);
        }
    }
}
