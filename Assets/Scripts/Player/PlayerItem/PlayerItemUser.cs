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
    ItemUserState userState;
    LineRenderer aimLine;

    InputAction ac_useItem, ac_hideItem, ac_stopUsing, ac_confirmUsage;

    public int UpdatePrio => 100;
    public bool IsAiming => userState == ItemUserState.Using;

    public static System.Action OnStartUsingItem;
    public static System.Action OnEndUsingItem;

    public void Init(PlayerContext context)
    {
        PlayerItemHolder.OnAddNewItem += OnAddNewItem;
        PlayerItemHolder.OnAddNewItem += OnAddNewItem;

        ac_useItem = new InputAction() { Stage = InputActionStage.ModeSpecific, Target = transform, Input = ControlType.Use, Text = "Use Item", ActionCallback = TryUseItem };
        ac_hideItem = new InputAction() { Stage = InputActionStage.ModeSpecific, Target = transform, Input = ControlType.Back, Text = "Hide Item", ActionCallback = () => SetUserState(ItemUserState.Hidden) };
        ac_stopUsing = new InputAction() { Stage = InputActionStage.ModeSpecific, Target = transform, Input = ControlType.Back, Text = "Stop", ActionCallback = () => SetUserState(ItemUserState.Idle) };
        ac_confirmUsage = new InputAction() { Stage = InputActionStage.ModeSpecific, Target = transform, Input = ControlType.Interact, Text = "Confirm", ActionCallback = UseConfirm };
    }

    private void OnAddNewItem(PlayerItem item, bool forceSelection)
    {
        if (forceSelection)
            OverrideSelectedItem(item);
    }

    private void TryUseItem()
    {
        if (selectedItem.IsUseable)
            SetUserState(ItemUserState.Using);
    }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        if (selectedItem && userState == ItemUserState.Using && aimLine != null)
        {
            selectedItem.AimUpdate(this, context, aimLine);
        }
    }

    private void UseEnter()
    {
        aimLine = gameObject.AddComponent<LineRenderer>();
        aimLine.useWorldSpace = true;
        aimLine.widthCurve = AnimationCurve.Constant(0, 1, 0.125f);
        aimLine.material = lineRendererMat;
    }

    private void UseConfirm()
    {
        PlayerItemUseResult useResult = selectedItem.AimInteract(this);

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

    private void UseExit()
    {
        Destroy(aimLine);
    }

    private void SetUserState(ItemUserState state)
    {
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform);

        if (userState == ItemUserState.Using)
            UseExit();

        userState = state;

        if (userState == ItemUserState.Idle)
        {
            PlayerInputActionRegister.Instance.RegisterInputAction(ac_useItem);
            PlayerInputActionRegister.Instance.RegisterInputAction(ac_hideItem);
        }

        if (state == ItemUserState.Using)
        {
            PlayerInputActionRegister.Instance.RegisterInputAction(ac_confirmUsage);
            PlayerInputActionRegister.Instance.RegisterInputAction(ac_stopUsing);
            UseEnter();
        }

        if (userState == ItemUserState.Hidden)
            OverrideSelectedItem(null, drop: true);

        Crosshair.SetMode(state == ItemUserState.Using ? Crosshair.Mode.Active : Crosshair.Mode.Passive);
    }

    internal void OverrideSelectedItem(PlayerItem item, bool drop = false)
    {
        if (item == null)
            OnEndUsingItem?.Invoke();
        else
        {
            OnStartUsingItem?.Invoke();
            SetUserState(ItemUserState.Idle);
        }

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
