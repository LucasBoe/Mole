using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : ScriptableObject
{
    public Sprite Sprite;
    public CollectablePlayerItem Prefab;
    public bool IsUseable;
    public float Force = 2;

    internal void AimUpdate(PlayerItemUser playerItemUser, PlayerContext context, LineRenderer aimLine)
    {
        Vector2 origin = (Vector2)aimLine.transform.position + Vector2.up;
        Vector2 dir = context.Input.VirtualCursorToDir(playerItemUser.transform.position);

        int visualizationPointCount = 100;
        float divisor = 10f;

        Vector2[] points = new Vector2[visualizationPointCount];

        for (int i = 0; i < visualizationPointCount; i++)
        {
            Vector2 point = origin + (dir * Force * i/ divisor + Vector2.down * Mathf.Pow(0.5f * 0.981f * i / divisor, 2));
            points[i] = point;
        }

        aimLine.positionCount = visualizationPointCount;
        aimLine.SetPositions(points.ToVector3Array());
    }

    internal bool AimInteract(PlayerContext context, PlayerItemUser playerItemUser)
    {
        Debug.LogWarning("Throw!");

        var playerPos = playerItemUser.transform.position;
        CollectablePlayerItem item = Instantiate(Prefab, playerPos + Vector3.up, Quaternion.identity);
        item.GetComponent<Rigidbody2D>().velocity = (context.Input.VirtualCursorToDir(playerPos) * Force * 5f);
        return true;
    }
}
