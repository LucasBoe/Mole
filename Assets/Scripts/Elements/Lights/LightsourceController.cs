using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightsourceController : AboveInputActionProvider
{
    [SerializeField] private ControllableLightsource lightsource;
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
        Log("TryInteract (on: " + lightsource.IsOn+ ")");
        lightsource.Switch();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, lightsource.transform.position);
    }
}
