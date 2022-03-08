using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ThrowablePlayerItem : PlayerItem
{
    public float ThrowForce = 2;
    public float GravityScale = 1;

    public override PlayerItemUseResult UseInteract()
    {
        return new PlayerItemUseResult(PlayerItemUseResult.Type.StartAim);
    }
    public override PlayerItemUseResult ConfirmInteract(PlayerItemUser playerItemUser, int activeModeIndex)
    {
        var playerPos = playerItemUser.transform.position;
        GameObject instance = Instantiate(GetObjectToInstatiate(), playerPos + Vector3.up, Quaternion.identity, LayerHandler.Parent);


        Rigidbody2D[] rigidbodys2D = instance.GetComponentsInChildren<Rigidbody2D>();

        foreach (Rigidbody2D rigidbody2D in rigidbodys2D)
        {
            rigidbody2D.velocity = (PlayerInputHandler.PlayerInput.VirtualCursorToDir(playerPos) * ThrowForce);
            rigidbody2D.gravityScale = GravityScale;
        }

        IThrowListener[] throwListeners = instance.GetComponents<IThrowListener>();
        foreach (IThrowListener listener in throwListeners)
            listener.OnThrow();

        return new PlayerItemUseResult(PlayerItemUseResult.Type.Destroy);
    }

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
            Vector2 point = origin + (dir * ThrowForce * t) + 0.5f * GravityScale * Physics2D.gravity * (t * t);
            points[i] = point;
        }

        aimLine.positionCount = visualizationPointCount;
        aimLine.SetPositions(points.ToVector3Array());
    }

    public virtual GameObject GetObjectToInstatiate()
    {
        return Prefab.gameObject;
    }

}
