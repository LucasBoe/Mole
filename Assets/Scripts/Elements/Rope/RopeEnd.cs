using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeEnd : AboveCooldownInteractable
{
    [SerializeField] protected Rigidbody2D rigidbody2D;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected Rope rope;

    public Rigidbody2D SetRope (Rope rope)
    {
        this.rope = rope;
        return rigidbody2D;
    }

    protected override void OnPlayerEnter()
    {
        //TODO: Replace with PlayerAboveInputActionProvider
        if (ShouldShowPrompt()) 
            PlayerInputActionRegister.Instance.RegisterInputAction(new InputAction() { Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Target = transform, ActionCallback = PlayerTryInteract });
    }

    protected override void OnPlayerExit()
    {
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(spriteRenderer);
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
