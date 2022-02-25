using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemUserState
{
    Hidden,
    Idle,
    Using,
}

public class PlayerItemUser : SingletonBehaviour<PlayerItemUser>, IPlayerComponent
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Material lineRendererMat;
    PlayerItem selectedItem;
    LineRenderer aimLine;

    public int UpdatePrio => 100;
    public bool IsAiming => aimLine != null;

    public static System.Action OnStartUsingItem;
    public static System.Action OnEndUsingItem;

    public void Init(PlayerContext context) { }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        if (selectedItem && IsAiming)
        {
            selectedItem.AimUpdate(this, context, aimLine);
        }
    }

    internal void Use(PlayerItem item)
    {
        aimLine = gameObject.GetComponent<LineRenderer>();
        if (aimLine == null)
            aimLine = gameObject.AddComponent<LineRenderer>();

        aimLine.useWorldSpace = true;
        aimLine.widthCurve = AnimationCurve.Constant(0, 1, 0.125f);
        aimLine.material = lineRendererMat;
        Crosshair.SetMode(Crosshair.Mode.Active);
    }

    internal void Confirm()
    {
        PlayerItemUseResult useResult = selectedItem.AimInteract(this);

        switch (useResult.ResultType)
        {
            case PlayerItemUseResult.Type.Destroy:
                Stop();
                PlayerItemHolder.Instance.RemoveItem(selectedItem);
                break;

            case PlayerItemUseResult.Type.Function:
                useResult.ResultFunction?.Invoke();
                break;
        }
    }

    internal void Stop()
    {
        Crosshair.SetMode(Crosshair.Mode.Passive);
        Destroy(aimLine);
    }

    internal void OverrideSelectedItem(PlayerItem item, bool drop = false)
    {
        if (selectedItem == item)
            return;

        if (selectedItem != null)
        {
            if (selectedItem.IsCarryable())
                (selectedItem as CarryPlayerItem).Carriable.SetCarryActive(false);

            if (drop && selectedItem.Prefab != null)
                Instantiate(selectedItem.Prefab, transform.position, Quaternion.identity);
        }

        spriteRenderer.sprite = (item != null && item.DisplaySprite()) ? item.Sprite : null;
        transform.rotation = Quaternion.identity;

        selectedItem = item;
    }
}
