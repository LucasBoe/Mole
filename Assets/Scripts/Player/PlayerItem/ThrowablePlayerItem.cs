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
        Vector2 inDir = context.Input.VirtualCursorToDir(playerItemUser.transform.position).normalized;

        Vector2[] points = Util.CalculateTrajectory(origin, inDir, ThrowForce, GravityScale * Physics2D.gravity);

        aimLine.positionCount = points.Length;
        aimLine.SetPositions(points.ToVector3Array());
    }

    public virtual GameObject GetObjectToInstatiate()
    {
        return Prefab.gameObject;
    }

}
