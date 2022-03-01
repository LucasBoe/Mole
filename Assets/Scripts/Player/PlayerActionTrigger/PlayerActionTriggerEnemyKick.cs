using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionTriggerEnemyKick : PlayerActionTriggerBase
{
    protected override InputAction CreateInputAction()
    {
        return new InputAction() { Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Text = "Kick", ActionCallback = Kick };
    }

    private void Kick()
    {
        if (ActionTarget == null)
            return;

        Rigidbody2D body = ActionTarget.GetComponent<Rigidbody2D>();

        if (body == null)
            return;

        Vector2 dir = (ActionTarget.position - transform.position) + Vector3.up;
        body.AddForce(dir.normalized * 50f, ForceMode2D.Impulse);
    }

    protected override bool ConditionsMet(Collider2D collider2D)
    {
        Vector2 position = collider2D.transform.position;
        Debug.Log(position);

        float dir = ((position.x < transform.position.x) ? -1f : 1f);
        Vector2 pos = position + new Vector2(dir * dir, -2f);
        Vector2 size = new Vector2(0.9f, 1.8f);
        bool isFree = Physics2D.OverlapBox(pos, size, 0, LayerMask.GetMask("Default")) == null;
        Util.DebugDrawBox(pos, size, isFree ? Color.green : Color.yellow, 10);
        return isFree;
    }
}
