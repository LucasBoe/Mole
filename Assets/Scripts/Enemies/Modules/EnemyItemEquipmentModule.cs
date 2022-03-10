using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemEquipmentModule : EnemyModule<EnemyItemEquipmentModule>
{
    [SerializeField] Transform hand;
    [SerializeField] GameObject lampPrefab;
    internal bool TryEquip(LampSource closestPotentialLamp)
    {
        Log("Try Equip!");
        Instantiate(lampPrefab, hand);
        closestPotentialLamp.Collect();
        return true;
    }
}
