using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemUser : PlayerSingletonBehaviour<PlayerItemUser>, IPlayerComponent
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Material lineRendererMat;
    [SerializeField, ReadOnly] PlayerItem selectedItem;

    private GameObject aimLineObject;
    LineRenderer aimLine;

    public int UpdatePrio => 100;
    public bool IsAiming => aimLine != null;

    public static System.Action OnStartUsingItem;
    public static System.Action OnEndUsingItem;

    public void Init(PlayerContext context)
    {
        aimLineObject = new GameObject();
        aimLineObject.transform.parent = transform;
        aimLineObject.transform.localPosition = Vector3.zero;
        aimLineObject.layer = LayerMask.NameToLayer("Pixelate");
    }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        if (selectedItem && IsAiming)
        {
            selectedItem.AimUpdate(this, context, aimLine);
        }
    }

    internal void Use(PlayerItem item)
    {
        PlayerItemUseResult useResult = selectedItem.UseInteract();
        ExecuteInteractionResult(useResult);
    }

    internal void Confirm(int selectedModeIndex)
    {
        PlayerItemUseResult confirmResult = selectedItem.ConfirmInteract(this, selectedModeIndex);
        ExecuteInteractionResult(confirmResult);
    }

    private void ExecuteInteractionResult(PlayerItemUseResult confirmResult)
    {
        switch (confirmResult.ResultType)
        {
            case PlayerItemUseResult.Type.Destroy:
                PlayerItemHolder.Instance.RemoveItem(selectedItem);
                break;

            case PlayerItemUseResult.Type.StartAim:

                aimLine = aimLineObject.GetComponent<LineRenderer>();
                if (aimLine == null)
                    aimLine = aimLineObject.AddComponent<LineRenderer>();

                aimLine.useWorldSpace = true;
                aimLine.widthCurve = AnimationCurve.Constant(0, 1, 0.125f);
                aimLine.material = lineRendererMat;
                Crosshair.SetMode(Crosshair.Mode.Active);
                break;

            case PlayerItemUseResult.Type.InCooldown:
                string str = confirmResult.ResultFloat.ToString("0.0");
                WorldTextSpawner.Spawn($"still in cooldown. remaining {str} s", transform.position);
                break;

            case PlayerItemUseResult.Type.Fail:
                if (!string.IsNullOrEmpty(confirmResult.ResultString))
                    WorldTextSpawner.Spawn(confirmResult.ResultString, transform.position);
                break;

            case PlayerItemUseResult.Type.Function:
                confirmResult.ResultFunction?.Invoke();
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
            {
                (selectedItem as CarryPlayerItem).Carriable.SetCarryActive(false);
                PlayerItemHolder.Instance.RemoveItem(selectedItem);
            }


            if (drop && selectedItem.Prefab != null)
                Instantiate(selectedItem.Prefab, transform.position, Quaternion.identity);
        }

        spriteRenderer.sprite = (item != null && item.DisplaySprite()) ? item.Sprite : null;
        transform.rotation = Quaternion.identity;

        selectedItem = item;
    }

    public bool IsHoldingHeavyItem()
    {
        return selectedItem != null && selectedItem.IsHeavy;
    }
}
