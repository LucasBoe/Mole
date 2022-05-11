using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCrank : RopeEnd
{
    Vector2 axisLast = Vector2.zero;
    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction() { Text = "Start Cranking", ActionCallback = StartCranking, Input = ControlType.Use, Stage = InputActionStage.WorldObject, Target = transform } };
    }

    private void StartCranking()
    {
        axisLast = Vector2.zero;
        PlayerInputHandler.OnPlayerInput += OnPlayerInput;
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform);
        PlayerInputActionRegister.Instance.RegisterInputAction(new InputAction() { Text = "Stop Cranking", ActionCallback = StopCranking, Input = ControlType.Back, Stage = InputActionStage.WorldObject, Target = transform });
    }

    private void StopCranking()
    {
        PlayerInputHandler.OnPlayerInput -= OnPlayerInput;
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform);
        if (playerIsAbove)
            OnPlayerEnter();
    }

    private void OnPlayerInput(PlayerInput input)
    {
        Vector2 axis = input.AxisRight;
        if (axisLast != Vector2.zero)
        {
            Vector2 pos = transform.position;
            Vector2 dir = axis;
            Vector2 perpendicular = Vector2.Perpendicular(dir);
            Debug.DrawLine(pos, pos + dir, Color.magenta);
            Debug.DrawLine(pos + dir, pos + dir + perpendicular, Color.yellow);

            Vector2 delta = axis - axisLast;
            float dot = Vector2.Dot(delta, perpendicular);

            if (rope != null)
                rope.Elongate(dot * 0.1f, distribution);
        }

        axisLast = axis;
    }
}
