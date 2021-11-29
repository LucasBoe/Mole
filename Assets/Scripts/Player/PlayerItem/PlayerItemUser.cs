using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemUserState
{
    Idle,
    Aim,
}

public class PlayerItemUser : MonoBehaviour, IPlayerComponent
{
    [SerializeField] SpriteRenderer spriteRenderer;
    PlayerItem inHand;
    ItemUserState userState;

    LineRenderer aimLine;

    public int UpdatePrio => 100;

    public void Init(PlayerContext context) { }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        if (inHand)
        {
            if (userState == ItemUserState.Aim)
            {
                AimUpdate(context);
            }
            else
            {
                if (context.Input.Use && inHand.IsUseable)
                {
                    SetUserState(ItemUserState.Aim);
                }
            }

            if (context.Input.Back)
            {
                if (userState == ItemUserState.Aim)
                    SetUserState(ItemUserState.Idle);
                else
                    SetItemInHand(null);
            }
        }
    }

    private void AimEnter()
    {
        aimLine = gameObject.AddComponent<LineRenderer>();
        aimLine.useWorldSpace = true;
        aimLine.widthCurve = AnimationCurve.Constant(0, 1, 0.2f);
    }
    private void AimUpdate(PlayerContext context)
    {
        if (context.Input.Interact && inHand.AimInteract(context ,this))
            SetItemInHand(null, drop: false);

        if (aimLine != null && inHand)
            inHand.AimUpdate(context, aimLine);

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
    }

    internal bool TryOverrideActiveItem(PlayerItem item)
    {
        SetItemInHand(item);
        return true;
    }

    private void SetItemInHand(PlayerItem item, bool drop = true)
    {
        if (inHand != null)
        {
            if (drop)
                Instantiate(inHand.Prefab, transform.position, Quaternion.identity);
            SetUserState(ItemUserState.Idle);
        }

        spriteRenderer.sprite = (item != null) ? item.Sprite : null;

        inHand = item;
    }
}
