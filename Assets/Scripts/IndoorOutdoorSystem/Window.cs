using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Replace with generalized system that allow for looking and entering separatly
public class Window : PlayerAboveInteractable
{
    [SerializeField] Layers targetLayer;
    [SerializeField] Sprite insideSprite, outsideSprite;

    private void Update()
    {
        if (playerIsAbove && PlayerInputHandler.PlayerInput.Interact)
        {
            LayerHandler.Instance.SetLayer(targetLayer);
        }
    }

    protected override void OnEnableWithPlayerAbove()
    {
        PlayerStateMachine.Instance.SetState(new InWindowState());
    }
}
