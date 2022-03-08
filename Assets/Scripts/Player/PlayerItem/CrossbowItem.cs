using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowItem : PlayerItem
{
    [SerializeField] private float projectileForce;
    [SerializeField] private float projectileGravityScale;
    [SerializeField] private CrossbowItemMode[] itemModes;

    private float lastUsedTime = float.MinValue;

    public override PlayerItemUseResult UseInteract()
    {
        lastUsedTime = float.MinValue;
        return new PlayerItemUseResult(type: PlayerItemUseResult.Type.StartAim);
    }

    public override void AimUpdate(PlayerItemUser playerItemUser, PlayerContext context, LineRenderer aimLine)
    {
        Vector2 origin = (Vector2)aimLine.transform.position + Vector2.up;
        Vector2 inDir = context.Input.VirtualCursorToDir(playerItemUser.transform.position).normalized;

        Vector2[] points = Util.CalculateTrajectory(origin, inDir, projectileForce, projectileGravityScale * Physics2D.gravity);

        aimLine.positionCount = points.Length;
        aimLine.SetPositions(points.ToVector3Array());
    }

    public override PlayerItemUseResult ConfirmInteract(PlayerItemUser playerItemUser, int activeModeIndex)
    {
        CrossbowItemMode mode = itemModes[activeModeIndex];
        float remainingCooldown = (lastUsedTime) - Time.time + mode.Cooldown;
        if (remainingCooldown > 0)
        {
            return new PlayerItemUseResult(type: PlayerItemUseResult.Type.InCooldown, remainingCooldown);
        }
        else
        {
            if (mode.ProjectileItem != null)
            {
                if (PlayerItemHolder.Instance.GetAmount(mode.ProjectileItem) == 0)
                    return new PlayerItemUseResult(type: PlayerItemUseResult.Type.Fail, "not enough bolts");
                else
                    PlayerItemHolder.Instance.RemoveItem(mode.ProjectileItem);
            }

            lastUsedTime = Time.time;
            var playerPos = playerItemUser.transform.position;
            var dir = PlayerInputHandler.PlayerInput.VirtualCursorToDir(playerPos);
            GameObject instance = Instantiate(mode.ProjectilePrefab, playerPos + Vector3.up, Quaternion.identity, LayerHandler.Parent);
            instance.transform.right = dir;
            instance.GetComponent<Rigidbody2D>().velocity = (dir * projectileForce);

            return new PlayerItemUseResult(PlayerItemUseResult.Type.None);
        }
    }

    public override ItemMode[] GetItemModes()
    {
        return itemModes;
    }
}

[System.Serializable]
public class CrossbowItemMode : ItemMode
{
    public PlayerItem ProjectileItem;
    public GameObject ProjectilePrefab;
    public float Cooldown;
}
