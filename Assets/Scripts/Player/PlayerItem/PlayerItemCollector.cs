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

    public int UpdatePrio => 100;

    public void Init(PlayerContext context)
    {
        //
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectablePlayerItem c = collision.GetComponent<CollectablePlayerItem>();
        if (c != null)
            items.Add(c);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollectablePlayerItem c = collision.GetComponent<CollectablePlayerItem>();
        if (c != null && items.Contains(c))
            items.Remove(c);
    }

    public void UpdatePlayerComponent(PlayerContext context)
    {
        if (context.Input.Interact && !itemUser.IsAiming && items.Count > 0)
        {
            CollectablePlayerItem firstItemInList = items[0];
            if (itemUser.TryOverrideActiveItem(firstItemInList.Item))
            {
                items.RemoveAt(0);
                Destroy(firstItemInList.gameObject);
            }
        }
    }
}
