using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopContentUI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText, priceText;
    [SerializeField] Button buyButton;

    private Shop shop;
    private ShopContent content;

    public void Init(Shop shop, ShopContent content)
    {
        this.shop = shop;
        this.content = content;
        iconImage.sprite = content.Sprite;
        nameText.text = content.Name;
        priceText.text = content.Price.ToString();
        buyButton.interactable = content.CouldBuy;
    }

    public void TryBuy ()
    {
        shop.TryBuy(content);
    }
}
