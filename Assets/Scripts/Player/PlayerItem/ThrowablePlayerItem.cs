using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowablePlayerItem : PlayerItem
{
    public float ThrowForce = 2;

    public override void AimUpdate(PlayerItemUser playerItemUser, PlayerContext context, LineRenderer aimLine)
    {
        Vector2 origin = (Vector2)aimLine.transform.position + Vector2.up;
        Vector2 dir = context.Input.VirtualCursorToDir(playerItemUser.transform.position);

        int visualizationPointCount = 100;
        float divisor = 10f;

        Vector2[] points = new Vector2[visualizationPointCount];

        for (int i = 0; i < visualizationPointCount; i++)
        {
            Vector2 point = origin + (dir * ThrowForce * i / divisor + Vector2.down * Mathf.Pow(0.5f * 0.981f * i / divisor, 2));
            points[i] = point;
        }

        aimLine.positionCount = visualizationPointCount;
        aimLine.SetPositions(points.ToVector3Array());
    }

    public override PlayerItemUseResult AimInteract(PlayerContext context, PlayerItemUser playerItemUser)
    {
        var playerPos = playerItemUser.transform.position;
        CollectablePlayerItem item = Instantiate(Prefab, playerPos + Vector3.up, Quaternion.identity);
        item.GetComponent<Rigidbody2D>().velocity = (context.Input.VirtualCursorToDir(playerPos) * ThrowForce * 5f);
        return new PlayerItemUseResult(PlayerItemUseResult.Type.Destroy);
    }

}
