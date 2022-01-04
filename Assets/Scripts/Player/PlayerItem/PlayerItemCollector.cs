using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerComponent
{
    int UpdatePrio { get; }
    void UpdatePlayerComponent(PlayerContext context);
    void Init(PlayerContext context);
}

public class PlayerItemCollector : MonoBehaviour, IPlayerComponent
{
    [SerializeField] PlayerItemUser itemUser;

    List<CollectablePlayerItem> items = new List<CollectablePlayerItem>();
    CollectablePlayerItem playerItem;

    public int UpdatePrio => 100;

    public void Init(PlayerContext context)
    {
        //
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectablePlayerItem c = collision.GetComponent<CollectablePlayerItem>();
        if (c != null)
        {
            PlayerInputActionRegister.Instance.RegisterInputAction(new InputAction() { Text = "Take " + c.Item.name, Object = c.SpriteRenderer, ActionCallback = TryCollect });
            playerItem = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollectablePlayerItem c = collision.GetComponent<CollectablePlayerItem>();
        if (c != null && PlayerInputActionRegister.Instance.UnregisterInputAction(c.SpriteRenderer)) playerItem = null;
    }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        //if (context.Input.Interact && !itemUser.IsAiming && items.Count > 0)
        //{
        //    CollectablePlayerItem firstItemInList = items[0];
        //    if (itemUser.TryOverrideActiveItem(firstItemInList.Item))
        //    {
        //        items.RemoveAt(0);
        //        Destroy(firstItemInList.gameObject);
        //    }
        //}
    }

    private void TryCollect()
    {
        if (playerItem != null && itemUser.TryOverrideActiveItem(playerItem.Item))
        {
            Destroy(playerItem.gameObject);
        }
    }
}
