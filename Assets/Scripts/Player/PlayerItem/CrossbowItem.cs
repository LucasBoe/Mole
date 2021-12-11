using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowItem : PlayerItem
{
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private float ProjectileForce;

    public override void AimUpdate(PlayerItemUser playerItemUser, PlayerContext context, LineRenderer aimLine)
    {
        Vector2 origin = playerItemUser.transform.position;

        Vector2 dir = context.Input.VirtualCursorToDir(origin);
        playerItemUser.transform.rotation = dir.ToRotation();

        aimLine.positionCount = 2;
        aimLine.SetPositions(new Vector3[] { origin, origin + (dir * 100) });
    }

    public override PlayerItemUseResult AimInteract(PlayerContext context, PlayerItemUser playerItemUser)
    {
        var playerPos = playerItemUser.transform.position;
        var dir = context.Input.VirtualCursorToDir(playerPos);
        GameObject instance = Instantiate(ProjectilePrefab, playerPos + Vector3.up, Quaternion.identity);
        instance.transform.right = dir;
        instance.GetComponent<Rigidbody2D>().velocity = (dir * ProjectileForce);

        return new PlayerItemUseResult(PlayerItemUseResult.Type.None);
    }
}
