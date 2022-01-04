using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelEntrance : PlayerAboveInteractable
{
    [SerializeField] Layers layerLeadsTo;
    PlayerControlPromptUI prompt;

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

    private void Update()
    {
        if (playerIsAbove)
        {
            if (PlayerInputHandler.PlayerInput.Interact)
            {
                enableTime = Time.time;
                TunnelUser.Instance.TrySetTunnelState(!TunnelUser.Instance.IsInTunnel);
            }
        }
    }
}
