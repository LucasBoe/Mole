using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionTriggerEnemyPull : PlayerActionTriggerBase
{
    protected override InputAction CreateInputAction()
    {
        return new InputAction() { Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Text = "Pull", ActionCallback = Pull };
    }

    private void Pull()
    {
        if (ActionTarget == null)
            return;

        Rigidbody2D body = ActionTarget.GetComponent<Rigidbody2D>();

        if (body == null)
            return;

        EnemyColliderModule colliderModule = ActionTarget.GetComponent<EnemyColliderModule>();

        if (colliderModule != null) colliderModule.SetCollisionActive(false);

        Vector2 dir = (transform.position - ActionTarget.position);
        body.AddForce(dir.normalized * 25f, ForceMode2D.Impulse);

        this.Delay(0.5f, () => { colliderModule.SetCollisionActive(true); });
    }

    protected override bool ConditionsMet(Collider2D collider2D)
    {
        return true;
    }
}
