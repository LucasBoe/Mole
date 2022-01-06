using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHolder : SingletonBehaviour<PlayerItemHolder>
{
    [SerializeField] private PlayerStartItemStack[] startItems;

    Dictionary<PlayerItem, int> itemAmountPairs = new Dictionary<PlayerItem, int>();

    public static System.Action<PlayerItem> OnAddNewItem;
    public static System.Action<PlayerItem> OnRemoveItem;

    private void Start()
    {
        foreach (PlayerStartItemStack pair in startItems)
        {
            AddItem(pair.Item, pair.Amount);
        }
    }

    public bool AddItem(PlayerItem item, int amount = 1)
    {
        if (itemAmountPairs.ContainsKey(item))
        {
            itemAmountPairs[item] += amount;
        }
        else
        {
            itemAmountPairs.Add(item, amount);
            OnAddNewItem?.Invoke(item);
        }

        return true;
    }

    public int RemoveItem(PlayerItem item, int toSubtract = 1)
    {
        if (itemAmountPairs.ContainsKey(item))
        {
            int currentAmount = itemAmountPairs[item];
            int newAmount = currentAmount - toSubtract;

            if (newAmount > 0)
            {
                itemAmountPairs[item] = newAmount;
                return toSubtract;
            } else
            {
                OnRemoveItem?.Invoke(item);
                itemAmountPairs.Remove(item);
                return toSubtract - newAmount;
            }
        }

        return 0;
    }
}

[System.Serializable]
public class PlayerStartItemStack
{
    public PlayerItem Item;
    public int Amount;
}