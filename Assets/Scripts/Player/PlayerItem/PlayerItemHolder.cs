using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHolder : SingletonBehaviour<PlayerItemHolder>
{
    [SerializeField] private PlayerStartItemStack[] startItems;

    private ItemSlotContainer items = new ItemSlotContainer();

    public static System.Action<PlayerItem, bool> OnAddNewItem;
    public static System.Action<PlayerItem> OnRemoveItem;

    private void Start()
    {
        foreach (PlayerStartItemStack pair in startItems)
        {
            AddItem(pair.Item, pair.Amount);
        }
    }
    public bool CanCollect(PlayerItem toCollect)
    {
        return true;
    }

    public bool AddItem(PlayerItem item, int amount = 1)
    {
        bool isHandItem = items.IsHandItem(item);

        if (items.ContainsItem(item) && !isHandItem)
        {
            items[item] += amount;
        }
        else
        {
            if (items.Add(item, amount))
            {
                OnAddNewItem?.Invoke(item, item.HandOnly);
            }
            else
                return false;
        }

        return true;
    }

    public int RemoveItem(PlayerItem item, int toSubtract = 1)
    {
        bool isHandItem = items.IsHandItem(item);

        if (items.ContainsItem(item) || isHandItem)
        {
            int currentAmount = items[item];
            int newAmount = currentAmount - toSubtract;

            if (newAmount <= 0 || isHandItem)
            {
                OnRemoveItem?.Invoke(item);
                items.Remove(item);
                return toSubtract - newAmount;
            } else
            {
                items[item] = newAmount;
                return toSubtract;
            }
        }

        return 0;
    }

    private class ItemSlotContainer
    {
        private PlayerItem handItem = null;
        private Dictionary<PlayerItem, int> playerItemAmountPairs = new Dictionary<PlayerItem, int>();

        public int this[PlayerItem item]
        {
            get
            {
                if (handItem == item)
                    return 1;
                else
                    return playerItemAmountPairs[item];
            }
            set { playerItemAmountPairs[item] = value; }
        }

        public bool Add(PlayerItem item, int amount)
        {
            if (item.HandOnly)
            {
                if (handItem == null)
                    handItem = item;
                else
                    return false;
            }
            else
            {
                playerItemAmountPairs.Add(item, amount);
            }

            return true;
        }

        public bool ContainsItem(PlayerItem item)
        {
            return playerItemAmountPairs.ContainsKey(item);
        }

        public void Remove(PlayerItem item)
        {
            if (handItem == item)
                handItem = null;
            else
                playerItemAmountPairs.Remove(item);
        }

        internal bool IsHandItem(PlayerItem item)
        {
            return item == handItem;
        }
    }
}

[System.Serializable]
public class PlayerStartItemStack
{
    public PlayerItem Item;
    public int Amount;
}