using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingState : PlayerStateBase
{
    bool transition = true;
    Vector2 startPosition;
    private ISlideable startSlideable;
    private InputAction cancelAction;

    public SlidingState(ISlideable currentSlideable)
    {
        this.startSlideable = currentSlideable;
        cancelAction = new InputAction() { ActionCallback = () => { SetState(new JumpState()); }, Input = ControlType.Back, Stage = InputActionStage.ModeSpecific, Target = PlayerController.Instance.transform, Text = "Cancel" };
    }
    public override void Enter()
    {
        transition = true;
        startPosition = startSlideable.GetClosestPosition(context.PlayerPos, context.Input.Axis);

        SetPlayerDragActive(false);
        SetCollisionActive(false);
        PlayerRopeSlider.Instance.SetSliderActive(true);
        PlayerInputActionRegister.Instance.RegisterInputAction(cancelAction);
    }

    public override void Update()
    {
        if (transition)
        {
            Vector2 currentPositon = context.PlayerPos;
            context.Rigidbody.MovePosition(Vector2.MoveTowards(currentPositon, startPosition, 1f));

            if (Vector2.Distance(currentPositon, startPosition) < 0.1f)
            {
                transition = false;
                SetCollisionActive(true);
            }
        }

        //var rigidbody = context.Rigidbody;
        //var velocity = rigidbody.velocity;
        //rigidbody.velocity = velocity.normalized * (Mathf.MoveTowards(velocity.magnitude, 10, Time.deltaTime * 100));
    }
    public override void Exit()
    {
        SetPlayerDragActive(true);
        PlayerRopeSlider.Instance.SetSliderActive(false);
        PlayerInputActionRegister.Instance.UnregisterInputAction(cancelAction);
    }
}
