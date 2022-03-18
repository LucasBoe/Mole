using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPullState : PlayerCombatState
{
    PlayerStateBase stateBefore;
    Rigidbody2D targetBody;
    bool pulled = false;

    public override bool CheckExit()
    {
        return pulled;
    }
    public EnemyPullState(ICombatTarget target, PlayerStateBase stateBefore) : base(target)
    {
        targetBody = target.Rigidbody2D;
        this.stateBefore = stateBefore;
    }

    public override void Enter()
    {
        pulled = false;
        target.CollisionModifier.SetCollisionActive(false);
        SetPlayerConstrained(true);
    }

    public override void Update()
    {
        float distance = Vector2.Distance(targetBody.position, context.PlayerPos);
        targetBody.MovePosition(Vector2.MoveTowards(targetBody.position, context.PlayerPos, 0.5f));
        pulled = distance < 0.25f;
        if (pulled)
            SetState(stateBefore);
    }

    public override void Exit()
    {
        target.CollisionModifier.SetCollisionActive(true);
        SetPlayerConstrained(false);
    }
}