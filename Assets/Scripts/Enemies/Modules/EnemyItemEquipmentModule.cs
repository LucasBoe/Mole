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
        inHand = hand.childCount > 0 ? EnemyHandItem.Lamp : EnemyHandItem.None;
    }

    public bool TryEquip(LampSource closestPotentialLamp)
    {
        Log("Try Equip!");
        Instantiate(lampPrefab_inHand, hand);
        closestPotentialLamp.Collect();
        inHand = EnemyHandItem.Lamp;
        return true;
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
