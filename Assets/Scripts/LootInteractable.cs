using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootInteractable : Interactable
{
    [SerializeField] LootContainer loot;

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction() { ActionCallback = Loot, Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Target = interactionOrigin, Text = "Take " + loot.GetName() } };
    }

    protected override void OnPlayerEnter()
    {
        if (loot.CanLoot)
            base.OnPlayerEnter();
    }

    public void Loot()
    {
        loot.PlayerTryLoot();
        OnPlayerExit();
    }
}
