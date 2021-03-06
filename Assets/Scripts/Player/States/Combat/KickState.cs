using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickState : PlayerCombatState
{
    Vector2 targetPos;
    Vector2 kickDirection;
    Rigidbody2D targetBody;
    bool finished = false;
    public bool Finished => finished;
    public bool DirectionIsRight => kickDirection.x > 0;
    public KickState(ICombatTarget target) : base(target)
    {
        targetBody = target.Rigidbody2D;
    }

    public override void Enter()
    {
        kickDirection = (targetBody.position - context.PlayerPos) + Vector2.up;
    }

    public override void Update()
    {
        if (!finished) targetPos = targetBody.position;

        context.Rigidbody.MovePosition(Vector2.MoveTowards(context.PlayerPos, targetPos, 1f));

        if (finished)
            return;

        float distance = context.Rigidbody.Distance(targetBody);
        if (distance < 0.1f)
        {
            Vector2 kick = kickDirection.normalized * 25f;
            target.Knock(kick);
            CameraShaker.Instance.Shake(targetPos, strength: 10f, frequency: 0.1f, duration: 0.5f);
            finished = true;
            SetStateDelayed(new IdleState(), 0.5f);
        }
    }
}
