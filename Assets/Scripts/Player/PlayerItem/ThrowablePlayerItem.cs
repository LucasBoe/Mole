using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowablePlayerItem : PlayerItem
{
    public float ThrowForce = 2;
    public float GravityScale = 1;

    public override void AimUpdate(PlayerItemUser playerItemUser, PlayerContext context, LineRenderer aimLine)
    {
        Vector2 origin = (Vector2)aimLine.transform.position + Vector2.up;
        Vector2 dir = context.Input.VirtualCursorToDir(playerItemUser.transform.position).normalized;

        int visualizationPointCount = 100;
        float divisor = 50f;

        Vector2[] points = new Vector2[visualizationPointCount];

        for (int i = 0; i < visualizationPointCount; i++)
        {
            float t = i / divisor;
            Vector2 point = origin + (dir * ThrowForce * t) + 0.5f * GravityScale * Physics2D.gravity * (t*t);
            points[i] = point;
        }

        aimLine.positionCount = visualizationPointCount;
        aimLine.SetPositions(points.ToVector3Array());
    }

    public override PlayerItemUseResult AimInteract(PlayerContext context, PlayerItemUser playerItemUser)
    {
        var playerPos = playerItemUser.transform.position;
        CollectablePlayerItem item = Instantiate(Prefab, playerPos + Vector3.up, Quaternion.identity);
        Rigidbody2D rigidbody2D = item.GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = (context.Input.VirtualCursorToDir(playerPos) * ThrowForce);
        rigidbody2D.gravityScale = GravityScale;
        return new PlayerItemUseResult(PlayerItemUseResult.Type.Destroy);
    }

}
