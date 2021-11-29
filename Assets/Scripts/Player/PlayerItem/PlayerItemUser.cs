using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemUser : MonoBehaviour, IPlayerComponent
{
    [SerializeField] SpriteRenderer spriteRenderer;
    PlayerItem inHand;

    public void UpdatePlayerComponent(PlayerContext context)
    {
        if (inHand && context.Input.Back)
            SetItemInHand(null);
    }

    internal bool TryOverrideActiveItem(PlayerItem item)
    {
        SetItemInHand(item);
        return true;
    }

    private void SetItemInHand(PlayerItem item)
    {
        if (inHand != null)
            Instantiate(inHand.Prefab, transform.position, Quaternion.identity);

        spriteRenderer.sprite = (item != null) ? item.Sprite : null;

        inHand = item;
    }
}
