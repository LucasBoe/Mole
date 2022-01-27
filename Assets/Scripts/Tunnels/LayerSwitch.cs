using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSwitch : PlayerAboveInteractable, IInputActionProvider
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Layers layerLeadsTo;
    PlayerControlPromptUI prompt;

    public InputAction FetchInputAction()
    {
        return new InputAction() { Text = "Enter", Target = transform, ActionCallback = TryInteract, Stage = InputActionStage.WorldObject };
    }

    protected override void OnPlayerEnter()
    {
        if (TunnelUser.Instance.IsInTunnel)
            LayerHandler.Instance.SetLayer(layerLeadsTo);
    }

    protected override void OnPlayerExit()
    {
        if (TunnelUser.Instance.IsInTunnel)
            LayerHandler.Instance.SetLayer(layerLeadsTo);
    }
    private void TryInteract()
    {
        enableTime = Time.time;
        TunnelUser.Instance.TrySetTunnelState(!TunnelUser.Instance.IsInTunnel);
    }
}
