using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

public class HidingState : PlayerStateBase
{
    Hideable hideable;
    Vector2 posBefore;
    Vector2 hideablePos;
    float distance;

    public HidingState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        base.Enter();


        hideable = Hideable.GetClosestFrom(GetCheck(CheckType.Hideable).Get<Hideable>(), context.PlayerPos);
        hideablePos = hideable.transform.position;
        posBefore = context.PlayerPos;
        distance = Vector2.Distance(posBefore, hideablePos);

        SetCollisionActive(false);
        SetGravityActive(false);
    }

    public override void Exit()
    {
        base.Exit();

        SetCollisionActive(true);
        SetGravityActive(true);
    }

    public override void Update()
    {
        Vector2 pos = Vector2.MoveTowards(context.PlayerPos, hideablePos, (distance * Time.deltaTime) / context.Values.SnapToHideablePositionDuration);
        context.Rigidbody.MovePosition(pos);

        if (context.Input.Back)
            SetState(PlayerState.Idle);
    }
}
