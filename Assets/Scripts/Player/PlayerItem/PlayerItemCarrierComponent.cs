using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCarrierComponent : PlayerSingletonBehaviour<PlayerItemCarrierComponent>
{
    [SerializeField] private new Rigidbody2D rigidbody2D;
    private CarriablePlayerItemWorldObject potCarryable, currentlyCarried;
    private InputAction carryAction = null;

    private void OnEnable()
    {
        CarriablePlayerItemWorldObject.OnStartCarry += OnStartCarry;
        CarriablePlayerItemWorldObject.OnEndCarry += OnEndCarry;
    }

    private void OnDisable()
    {
        CarriablePlayerItemWorldObject.OnStartCarry -= OnStartCarry;
        CarriablePlayerItemWorldObject.OnEndCarry -= OnEndCarry;
    }

    private void OnEndCarry(CarriablePlayerItemWorldObject worldObject)
    {
        if (currentlyCarried == worldObject)
            currentlyCarried = null;
    }

    private void OnStartCarry(CarriablePlayerItemWorldObject worldObject)
    {
        currentlyCarried = worldObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CarriablePlayerItemWorldObject c = collision.GetComponent<CarriablePlayerItemWorldObject>();

        if (CouldCarry(c))
        {
            potCarryable = c;
            carryAction = potCarryable.GetCarryAction();
            PlayerInputActionRegister.Instance.RegisterInputAction(carryAction);
        }
    }

    private bool CouldCarry(CarriablePlayerItemWorldObject c)
    {
        if (c == null || c == currentlyCarried)
            return false;

        if (c.Item.IsHeavy && !PlayerStateMachine.Instance.CurrentState.StateAllowsCarryingHeavyObjects)
            return false;

        return PlayerItemHolder.Instance.CanCollect(c.Item);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        CarriablePlayerItemWorldObject c = collision.GetComponent<CarriablePlayerItemWorldObject>();

        if (c != null && PlayerInputActionRegister.Instance.UnregisterAllInputActions(c.transform)) potCarryable = null;
    }

    public void TryCarry(CarriablePlayerItemWorldObject carriablePlayerItemWorldObject)
    {
        if (CouldCarry(carriablePlayerItemWorldObject))
        {
            if (carryAction != null)
                PlayerInputActionRegister.Instance.UnregisterInputAction(carryAction);

            if (currentlyCarried != null)
                PlayerItemHolder.Instance.RemoveItem(currentlyCarried.Item);

            potCarryable.StartCarry(rigidbody2D);
        }
        else if (potCarryable != null)
        {
            PlayerInputActionRegister.Instance.UnregisterAllInputActions(potCarryable.transform);
            potCarryable = null;
        }
    }
}
