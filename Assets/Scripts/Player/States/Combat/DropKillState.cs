using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropKillState : PlayerCombatState
{
    Rigidbody2D targetBody;
    float startTime;
    const float moveXtoTargetForce = 10;
    public DropKillState(ICombatTarget target) : base(target)
    {
        targetBody = target.Rigidbody2D;
    }

    public override void Enter()
    {
        SetCollisionActive(false);
        startTime = Time.time;
    }

    public override void Update()
    {
        float xPos = Mathf.MoveTowards(context.PlayerPos.x, targetBody.position.x, Time.captureDeltaTime * moveXtoTargetForce);
        float yDistance = Mathf.Abs(context.PlayerPos.y - targetBody.position.y);

        if (yDistance < 1f)
        {
            target.Kill();
            SetState(new IdleState());
        }

        context.Rigidbody.position = new Vector2(xPos, context.Rigidbody.position.y);
        ApplyGravity(Time.time - startTime);
    }

    public override void Exit()
    {
        SetCollisionActive(true);
    }
}
