using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCarrierComponent : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody2D;
    CarriablePlayerItemWorldObject carryable;
    InputAction current = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CarriablePlayerItemWorldObject c = collision.GetComponent<CarriablePlayerItemWorldObject>();
        if (c != null)
        {
            //TODO: Move this to input action register look at PlayerAboveInputActionProvider
            current = new InputAction() { Text = "Carry " + c.Item.name, Target = transform, Stage = InputActionStage.WorldObject, ActionCallback = TryCarry };
            PlayerInputActionRegister.Instance.RegisterInputAction(current);
            carryable = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollectablePlayerItemWorldObject c = collision.GetComponent<CollectablePlayerItemWorldObject>();
        if (c != null && PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform)) carryable = null;
    }

    private void TryCarry()
    {
        if (carryable != null && PlayerItemHolder.Instance.CanCollect(carryable.Item))
        {
            if (current != null)
                PlayerInputActionRegister.Instance.UnregisterInputAction(current);

            carryable.StartCarry(rigidbody2D);
        }
    }
}
