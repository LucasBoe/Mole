using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hideable : PlayerAboveInteractable, IInputActionProvider
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    //PlayerControlPromptUI promptUI;
    protected override void OnPlayerEnter()
    {
        //promptUI = PlayerControlPromptUI.Show(ControlType.Use, transform.position + (Vector3.up));
    }

    protected override void OnPlayerExit()
    {
        //if (promptUI != null) promptUI.Hide();
    }

    internal static Hideable GetClosestFrom(Hideable[] hideables, Vector2 playerPos)
    {
        return hideables.OrderBy(h => Vector2.Distance(h.transform.position, playerPos)).First();
    }

    public InputAction FetchInputAction()
    {
        return new InputAction() { Object = spriteRenderer, Text = "Hide", ActionCallback = Hide };
    }

    private void Hide()
    {
        PlayerStateMachine.Instance.SetState(PlayerState.Hiding);
    }
}
