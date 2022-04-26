using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] UIBlender blender;
    [SerializeField] ShopContentUI shopContentUI;
    private void Awake()
    {
        shopContentUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Shop.OpenShop += OnOpenShop;
    }

    private void OnOpenShop(Shop shop)
    {
        blender.SetActive(true);
        shopContentUI.transform.parent.DestroyAllChildrenExcept(shopContentUI.transform);

        foreach (ShopContent content in shop.Content)
        {
            ShopContentUI newUI = Instantiate(shopContentUI, shopContentUI.transform.parent);
            newUI.gameObject.SetActive(true);
            newUI.Init(shop, content);
        }
    }
}
