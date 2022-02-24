using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriablePlayerItemWorldObject : InteractablePlayerItemWorldObject
{
    [SerializeField] DistanceJoint2D distanceJoint2D;

    public void SetCarryActive(bool carry)
    {
        distanceJoint2D.enabled = carry;
    }

    internal void StartCarry(Rigidbody2D targetBody)
    {
        distanceJoint2D.connectedBody = targetBody;
        SetCarryActive(true);
        CarryPlayerItem carry = Item as CarryPlayerItem;
        carry.Carriable = this;
        PlayerItemHolder.Instance.AddItem(carry);
    }
}
