using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeEnd : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rigidbody2D;
    [SerializeField] protected bool playerIsAbove = false;

    protected Rope rope;
    protected PlayerControlPromptUI prompt;

    public Rigidbody2D SetRope (Rope rope)
    {
        this.rope = rope;
        return rigidbody2D;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
         if (!collision.IsPlayer())
            return;

        if (ShouldShowPrompt())
            prompt = PlayerControlPromptUI.Show(ControlType.Interact, transform.position + Vector3.up);

        playerIsAbove = true;
    }

    protected virtual bool ShouldShowPrompt()
    {
        return rope != null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        if (prompt != null) prompt.Hide();
        playerIsAbove = false;

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
