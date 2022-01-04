using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelEntrance : PlayerAboveInteractable
{
    [SerializeField] Layers layerLeadsTo;
    PlayerControlPromptUI prompt;

    protected override void OnPlayerEnter()
    {
        prompt = PlayerControlPromptUI.Show(ControlType.Interact, transform.position + (Vector3.up + Vector3.right));
        if (TunnelUser.Instance.IsInTunnel)
            LayerHandler.Instance.SetLayer(layerLeadsTo);
    }

    protected override void OnPlayerExit()
    {
        if (prompt != null) prompt.Hide();

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
