using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootInteractable : AboveInputActionProvider
{
    [SerializeField] LootContainer loot;

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction() { ActionCallback = Loot, Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Target = transform, Text = loot.GetName() } };
    }

    protected override void OnPlayerEnter()
    {
        if (loot.CanLoot)
            base.OnPlayerEnter();
    }

    private void Loot()
    {
        loot.PlayerTryLoot();
        OnPlayerExit();
    }
}
