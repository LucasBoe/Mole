using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHolder : PlayerSingletonBehaviour<PlayerItemHolder>
{
    [SerializeField] private PlayerStartItemStack[] startItems;

    private ItemSlotContainer items = new ItemSlotContainer();

    public static System.Action<PlayerItem> AddedItem;
    public static System.Action<PlayerItem> Changedtem;
    public static System.Action<PlayerItem> RemovedItem;

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
            Changedtem?.Invoke(item);
        }
        else
        {
            if (items.Add(item, amount))
                AddedItem?.Invoke(item);
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
                items.Remove(item);
                RemovedItem?.Invoke(item);
                return toSubtract - newAmount;
            }
            else
            {
                items[item] = newAmount;
                Changedtem?.Invoke(item);
                return toSubtract;
            }
        }

        return 0;
    }

    public int GetAmount(PlayerItem item)
    {
        if (items.ContainsItem(item))
            return items.Pairs[item];

        return 0;
    }

    private class ItemSlotContainer
    {
        private PlayerItem handItem = null;
        private Dictionary<PlayerItem, int> playerItemAmountPairs = new Dictionary<PlayerItem, int>();
        public Dictionary<PlayerItem, int> Pairs => playerItemAmountPairs;

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
            playerItemAmountPairs.Add(item, amount);
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

        public bool IsHandItem(PlayerItem item)
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