using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hideable : PlayerAboveInputActionProvider
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    internal static Hideable GetClosestFrom(Hideable[] hideables, Vector2 playerPos)
    {
        return hideables.OrderBy(h => Vector2.Distance(h.transform.position, playerPos)).First();
    }

    protected override InputAction CreateInputAction()
    {
        return new InputAction() { Input = ControlType.Use, Target = transform, Text = "Hide", ActionCallback = Hide };
    }

    private void Hide()
    {
        PlayerStateMachine.Instance.SetState(PlayerState.Hiding);
    }
}
