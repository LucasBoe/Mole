using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUtil : MonoBehaviour
{
    public static int GetMoney()
    {
        return PlayerItemHolder.Instance.GetAmount(PlayerController.Context.Values.CurrencyItem);
    }

    internal static void RemoveMoney(int price)
    {
        PlayerItem currency = PlayerController.Context.Values.CurrencyItem;
        for (int i = 0; i < price; i++)
        {
            PlayerItemHolder.Instance.RemoveItem(currency);
        }
    }

    public static Vector2 Position => PlayerController.Context.PlayerPos;
}
