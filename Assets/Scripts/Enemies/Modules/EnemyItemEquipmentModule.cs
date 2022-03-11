using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemEquipmentModule : EnemyModule<EnemyItemEquipmentModule>
{
    [SerializeField] Transform hand;
    [SerializeField] GameObject lampPrefab_inHand, lampPrefab_drop;
    [SerializeField, ReadOnly] EnemyHandItem inHand = EnemyHandItem.None;

    private void Start()
    {
        inHand = CheckItemInHand();
    }

    private EnemyHandItem CheckItemInHand()
    {
        if (hand.childCount > 0)
        {
            if (hand.GetChild(0).gameObject.activeSelf)
                return EnemyHandItem.Lamp;
            else
                hand.DestroyAllChildren();
        }

        return EnemyHandItem.None;
    }

    public bool TryEquip(LampSource closestPotentialLamp)
    {
        Log("Try Equip!");
        if (closestPotentialLamp.Collect())
        {
            Instantiate(lampPrefab_inHand, hand);
            inHand = EnemyHandItem.Lamp;
            return true;
        }
        else
            return false;
    }

    public void DropItem()
    {
        Log("Drop!");

        switch (inHand)
        {
            case EnemyHandItem.Lamp:
                hand.DestroyAllChildren();
                Instantiate(lampPrefab_drop, hand.position, Quaternion.identity, LayerHandler.Parent);
                break;
        }

        inHand = EnemyHandItem.None;
    }

    private enum EnemyHandItem
    {
        None,
        Lamp,
    }
}
