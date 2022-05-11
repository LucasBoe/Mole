using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeEnd : AboveInputActionProvider
{
    [SerializeField] new protected Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D => rigidbody2D;

    protected Rope rope;
    protected float distribution;


    public Rigidbody2D SetRope(Rope rope, float distribution)
    {
        this.rope = rope;
        this.distribution = distribution;
        return rigidbody2D;
    }

    protected override void OnPlayerEnter()
    {
        if (ShouldShowPrompt())
            base.OnPlayerEnter();
    }

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction() { Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Target = transform, ActionCallback = PlayerTryInteract, Text = "Take Rope" }};
    }

    protected virtual bool ShouldShowPrompt()
    {
        return rope != null;
    }

    protected virtual void PlayerTryInteract()
    {
        PlayerRopeUser ropeUser = PlayerRopeUser.Instance;
        if (rope != null && !ropeUser.IsActive)
        {
            ropeUser.TakeRopeFrom(rope, rigidbody2D);
            rope = null;
            Destroy(gameObject);
        }
    }
}
