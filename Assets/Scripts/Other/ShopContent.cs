using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopContent
{
    [SerializeField] PlayerItem item;
    public Sprite Sprite => item.Sprite;
    public string Name => item.name;
    public int Price;
    public bool CouldBuy => Price <= PlayerUtil.GetMoney();

    internal void Buy()
    {
        PlayerUtil.RemoveMoney(Price);
        PlayerItemHolder.Instance.AddItem(item);
    }
}
