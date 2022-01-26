using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerComponent
{
    //the bigger the number the earlier the update
    int UpdatePrio { get; }
    void UpdatePlayerComponent(PlayerContext context);
    void Init(PlayerContext context);

}

public class PlayerItemCollector : MonoBehaviour
{
    CollectablePlayerItem playerItem;
    InputAction current = null;

    public int UpdatePrio => 100;

    public void Init(PlayerContext context)    {    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectablePlayerItem c = collision.GetComponent<CollectablePlayerItem>();
        if (c != null)
        {
            current = new InputAction() { Text = "Take " + c.Item.name, Object = c.SpriteRenderer, ActionCallback = TryCollect };
            PlayerInputActionRegister.Instance.RegisterInputAction(current);
            playerItem = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollectablePlayerItem c = collision.GetComponent<CollectablePlayerItem>();
        if (c != null && PlayerInputActionRegister.Instance.UnregisterInputAction(c.SpriteRenderer)) playerItem = null;
    }

    private void TryCollect()
    {
        if (playerItem != null && PlayerItemHolder.Instance.AddItem(playerItem.Item))
        {
            if (current != null)
                PlayerInputActionRegister.Instance.UnregisterInputAction(current);

            Destroy(playerItem.gameObject);
        }
    }
}
