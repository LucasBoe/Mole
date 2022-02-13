using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInputActionCreator
{
    private static InputAction ClimbRopeAction = new InputAction() { ActionCallback = () => PlayerStateMachine.Instance.SetState(new RopeClimbState()) , Input = ControlType.Use, Stage = InputActionStage.WorldObject, Text = "Climb Rope" };
    public static InputAction GetClimbRopeAction( Transform target )
    {
        ClimbRopeAction.Target = target;
        return ClimbRopeAction;
    }
}
