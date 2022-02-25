using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCarrierComponent : SingletonBehaviour<PlayerItemCarrierComponent>
{
    [SerializeField] Rigidbody2D rigidbody2D;
    CarriablePlayerItemWorldObject potCarryable, currentlyCarried;
    InputAction carryAction = null;

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

        if (c != null && c != currentlyCarried)
        {
            potCarryable = c;
            carryAction = potCarryable.GetCarryAction();
            PlayerInputActionRegister.Instance.RegisterInputAction(carryAction);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        CarriablePlayerItemWorldObject c = collision.GetComponent<CarriablePlayerItemWorldObject>();

        if (c != null && PlayerInputActionRegister.Instance.UnregisterAllInputActions(c.transform)) potCarryable = null;
    }

    public void TryCarry(CarriablePlayerItemWorldObject carriablePlayerItemWorldObject)
    {
        if (potCarryable != null && currentlyCarried != potCarryable && PlayerItemHolder.Instance.CanCollect(potCarryable.Item))
        {
            if (carryAction != null)
                PlayerInputActionRegister.Instance.UnregisterInputAction(carryAction);

            if (currentlyCarried != null)
                PlayerItemHolder.Instance.RemoveItem(currentlyCarried.Item);

            potCarryable.StartCarry(rigidbody2D);
        }
    }
}
