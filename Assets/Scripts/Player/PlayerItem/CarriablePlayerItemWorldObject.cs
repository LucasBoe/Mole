using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriablePlayerItemWorldObject : InteractablePlayerItemWorldObject
{
    public static System.Action<CarriablePlayerItemWorldObject> OnStartCarry, OnEndCarry;

    [SerializeField] DistanceJoint2D distanceJoint2D;

    private InputAction carryAction;

    private void Start()
    {
        carryAction = new InputAction() { Text = "Carry " + Item.name, Target = transform, Stage = InputActionStage.WorldObject, ActionCallback = () => PlayerItemCarrierComponent.Instance.TryCarry(this) };
    }

    public void SetCarryActive(bool carry)
    {
        distanceJoint2D.enabled = carry;
        (carry ? OnStartCarry : OnEndCarry)?.Invoke(this);
    }

    internal void StartCarry(Rigidbody2D targetBody)
    {
        distanceJoint2D.connectedBody = targetBody;
        SetCarryActive(true);
        CarryPlayerItem carry = Item as CarryPlayerItem;
        carry.Carriable = this;
        PlayerItemHolder.Instance.AddItem(carry);
    }

    internal InputAction GetCarryAction()
    {
        return carryAction;
    }
}
