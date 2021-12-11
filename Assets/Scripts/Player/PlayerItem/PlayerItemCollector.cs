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

    CollectablePlayerItem item;

    public int UpdatePrio => 100;

    public void Init(PlayerContext context)
    {
        //
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectablePlayerItem c = collision.GetComponent<CollectablePlayerItem>();
        if (c != null)
            item = c;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollectablePlayerItem c = collision.GetComponent<CollectablePlayerItem>();
        if (c != null && c == item)
            item = null;
    }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        //use jump input as collection action
        if (context.Input.Interact && item && itemUser.TryOverrideActiveItem(item.Item))
        {
            Destroy(item.gameObject);
        }
    }
}
