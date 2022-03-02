using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPullState : PlayerCombatState
{
    PlayerStateBase stateBefore;
    Rigidbody2D targetBody;
    public EnemyPullState(ICombatTarget target, PlayerStateBase stateBefore) : base(target)
    {
        targetBody = target.Rigidbody2D;
        this.stateBefore = stateBefore;
    }

    public override void Enter()
    {
        target.ColliderModule.SetCollisionActive(false);
    }

    public override void Update()
    {
        float distance = Vector2.Distance(targetBody.position, context.PlayerPos);
        targetBody.MovePosition(Vector2.MoveTowards(targetBody.position, context.PlayerPos, Time.deltaTime * 20f));

        if (distance < 0.1f)
            SetState(stateBefore);
    }

    public override void Exit()
    {
        target.ColliderModule.SetCollisionActive(false);
    }
}