using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowItem : PlayerItem
{
    [SerializeField] private float ProjectileForce;
    [SerializeField] private CrossbowItemMode[] itemModes;

    private float lastUsedTime;

    public override void AimUpdate(PlayerItemUser playerItemUser, PlayerContext context, LineRenderer aimLine)
    {
        Vector2 origin = playerItemUser.transform.position;

        Vector2 dir = context.Input.VirtualCursorToDir(origin);
        playerItemUser.transform.rotation = dir.ToRotation();

        aimLine.positionCount = 2;
        aimLine.SetPositions(new Vector3[] { origin, origin + (dir * 100) });
    }

    public override PlayerItemUseResult ConfirmInteract(PlayerItemUser playerItemUser, int activeModeIndex)
    {
        CrossbowItemMode mode = itemModes[activeModeIndex];
        float remainingCooldown = (lastUsedTime + mode.Cooldown) - Time.time;
        if (remainingCooldown > 0)
        {
            return new PlayerItemUseResult(type: PlayerItemUseResult.Type.InCooldown, remainingCooldown);
        }
        else
        {
            lastUsedTime = Time.time;
            var playerPos = playerItemUser.transform.position;
            var dir = PlayerInputHandler.PlayerInput.VirtualCursorToDir(playerPos);
            GameObject instance = Instantiate(mode.ProjectilePrefab, playerPos + Vector3.up, Quaternion.identity, LayerHandler.Parent);
            instance.transform.right = dir;
            instance.GetComponent<Rigidbody2D>().velocity = (dir * ProjectileForce);

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
    public GameObject ProjectilePrefab;
    public float Cooldown;
}
