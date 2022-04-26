using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOETrapDefusionInteractable : AboveInputActionProvider
{
    [SerializeField] AOETrap trap;
    [SerializeField] LootContainer aoeTrapItemContainer;
    [SerializeField] PlayerItem requiredItem;

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction() { ActionCallback = TryInteract, Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Target = transform, Text = "Defuse" } };
    }

    public void TryInteract()
    {
        if (requiredItem != null)
        {
            if (PlayerItemHolder.Instance.GetAmount(requiredItem) == 0)
            {
                WorldTextSpawner.Spawn(requiredItem.Sprite, "0/1 " + requiredItem.name, transform.position);
                return;
            }
            else
            {
                PlayerItemHolder.Instance.RemoveItem(requiredItem);
            }
        }

        StartCoroutine(DefusionRoutine());
    }

    private IEnumerator DefusionRoutine()
    {
        PlayerActionProgressionVisualizerUI ui = UIUtil.SpawnActionProgressionVisualizer();
        float t = 0, duration = 1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            ui.UpdateValue(t / duration);
            yield return null;
        }

        ui.Hide();

        if (aoeTrapItemContainer.PlayerTryLoot())
            Destroy(trap.gameObject);
        
    }
}
