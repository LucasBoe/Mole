using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemUserState
{
    Hidden,
    Idle,
    Aim,
}

public class PlayerItemUser : SingletonBehaviour<PlayerItemUser>, IPlayerComponent
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Material lineRendererMat;
    PlayerItem selectedItem;
    ItemUserState userState;
    LineRenderer aimLine;

    public int UpdatePrio => 100;
    public bool IsAiming => userState == ItemUserState.Aim;

    public static System.Action OnStartUsingItem;
    public static System.Action OnEndUsingItem;

    public void Init(PlayerContext context)
    {
        PlayerItemHolder.OnAddNewItem += OnAddNewItem;
        PlayerItemHolder.OnAddNewItem += OnAddNewItem;
    }

    private void OnAddNewItem(PlayerItem item, bool forceSelection)
    {
        if (forceSelection)
            OverrideSelectedItem(item);
    }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        if (selectedItem)
        {
            if (userState == ItemUserState.Aim)
            {
                AimUpdate(context);
            }
            else
            {
                if (context.Input.Use && selectedItem.IsUseable)
                {
                    SetUserState(ItemUserState.Aim);
                }
            }

            if (context.Input.Back)
            {
                if (userState == ItemUserState.Aim)
                    SetUserState(ItemUserState.Idle);
                else
                    SetUserState(ItemUserState.Hidden);
            }
        }
    }

    private void AimEnter()
    {
        aimLine = gameObject.AddComponent<LineRenderer>();
        aimLine.useWorldSpace = true;
        aimLine.widthCurve = AnimationCurve.Constant(0, 1, 0.125f);
        aimLine.material = lineRendererMat;
    }
    private void AimUpdate(PlayerContext context)
    {
        if (context.Input.Interact)
        {
            PlayerItemUseResult useResult = selectedItem.AimInteract(context, this);

            switch (useResult.ResultType)
            {
                case PlayerItemUseResult.Type.Destroy:
                    PlayerItemHolder.Instance.RemoveItem(selectedItem);
                    SetUserState(ItemUserState.Hidden);
                    break;

                case PlayerItemUseResult.Type.Function:
                    useResult.ResultFunction?.Invoke();
                    break;
            }
        }

        if (aimLine != null && selectedItem)
            selectedItem.AimUpdate(this, context, aimLine);
    }
    private void AimExit()
    {
        Destroy(aimLine);
    }

    private void SetUserState(ItemUserState state)
    {
        if (userState == ItemUserState.Aim)
            AimExit();

        userState = state;

        if (state == ItemUserState.Aim)
            AimEnter();

        if (userState == ItemUserState.Hidden)
            OverrideSelectedItem(null, drop: true);

        Crosshair.SetMode(state == ItemUserState.Aim ? Crosshair.Mode.Active : Crosshair.Mode.Passive);
    }

    internal void OverrideSelectedItem(PlayerItem item, bool drop = false)
    {
        if (item == null)
            OnEndUsingItem?.Invoke();
        else
            OnStartUsingItem?.Invoke();

        if (selectedItem == item)
            return;

        SetSelectedItem(item, drop);
    }

    private void SetSelectedItem(PlayerItem item, bool drop)
    {
        if (selectedItem != null)
        {
            if (drop && selectedItem.Prefab != null)
                Instantiate(selectedItem.Prefab, transform.position, Quaternion.identity);
        }


        spriteRenderer.sprite = (item != null) ? item.Sprite : null;

        selectedItem = item;
    }
}
