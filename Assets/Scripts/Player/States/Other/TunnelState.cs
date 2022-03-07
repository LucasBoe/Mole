using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System;

public class TunnelState : PlayerStateBase
{
    public Vector2Int MoveDir; 
    public TunnelState(Transform switchTransform)
    {
        context.Rigidbody.transform.position = switchTransform.position;
    }

    public override void Enter()
    {
        SetGravityActive(false);
    }

    public override void Update()
    {
        Vector2 axis = context.Input.Axis;

        float x = Mathf.Clamp(axis.x, RaycastForWall(context.PlayerPos, Vector2.left) ? 0 : -1, RaycastForWall(context.PlayerPos, Vector2.right) ? 0 : 1);
        float y = Mathf.Clamp(axis.y, RaycastForWall(context.PlayerPos, Vector2.down) ? 0 : -1, RaycastForWall(context.PlayerPos, Vector2.up) ? 0 : 1);
        axis = new Vector2(x, y);

        if (axis == Vector2.zero)
            MoveDir = Vector2Int.zero;
        else if (Mathf.Abs(axis.x) > Mathf.Abs(axis.y))
            MoveDir = axis.x >= 0 ? Vector2Int.right : Vector2Int.left;
        else
            MoveDir = axis.y >= 0 ? Vector2Int.up : Vector2Int.down;

        Vector2 target = AdaptToTunnelCoordinates(context.PlayerPos + (axis * 3));
        Vector2 withTime = Vector2.MoveTowards(context.PlayerPos, target, Time.deltaTime * context.Values.TunnelMoveVelocity);
        context.Rigidbody.MovePosition(withTime);
    }

    private bool RaycastForWall(Vector2 origin, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1, LayerMask.GetMask("Default"));
        bool hasHit = hit.collider != null;
        Watch(direction.ToString(), hasHit.ToString());
        Debug.DrawLine(origin, hasHit ? hit.point : origin + direction, hasHit ? Color.red : Color.green);
        return hasHit;
    }

    private Vector2 AdaptToTunnelCoordinates(Vector2 vector2)
    {
        Vector2 zeroPointFive = new Vector2(0.5f, 0.5f);
        return (vector2 + zeroPointFive).Round() - zeroPointFive;
    }

    public override void Exit()
    {
        SetGravityActive(true);
    }
}
