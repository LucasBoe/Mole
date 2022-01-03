using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeEnd : PlayerAboveInteractable
{
    [SerializeField] protected Rigidbody2D rigidbody2D;

    protected Rope rope;
    protected PlayerControlPromptUI prompt;

    public Rigidbody2D SetRope (Rope rope)
    {
        this.rope = rope;
        return rigidbody2D;
    }

    protected override void OnPlayerEnter()
    {
        if (ShouldShowPrompt())
            prompt = PlayerControlPromptUI.Show(ControlType.Interact, transform.position + Vector3.up);
    }

    protected override void OnPlayerExit()
    {
        if (prompt != null) prompt.Hide();
    }

    protected virtual bool ShouldShowPrompt()
    {
        return rope != null;
    }

    private void Update()
    {
        if (playerIsAbove && PlayerInputHandler.PlayerInput.Interact)
        {
            PlayerRopeUser ropeUser = PlayerRopeUser.Instance;
            PlayerTryInteract(ropeUser);
        }
    }

    protected virtual void PlayerTryInteract(PlayerRopeUser ropeUser)
    {
        if (rope != null && !ropeUser.IsActive)
        {
            ropeUser.TakeRopeFrom(rope, rigidbody2D);
            rope = null;
            Destroy(gameObject);
        }
    }
}
