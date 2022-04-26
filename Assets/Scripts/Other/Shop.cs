using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] List<ShopContent> contents = new List<ShopContent>();

    public static System.Action<Shop> OpenShop;

    public List<ShopContent> Content => contents;

    internal void TryBuy(ShopContent content)
    {
        if (!contents.Contains(content)) return;
        if (!content.CouldBuy)  return;

        content.Buy();
        Open();
    }

    [ContextMenu("Open Shop")]
    public void Open()
    {
        OpenShop?.Invoke(this);
    }
}
