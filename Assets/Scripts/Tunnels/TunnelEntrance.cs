using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelEntrance : PlayerAboveInteractable
{
    PlayerControlPromptUI prompt;
    protected override void OnPlayerEnter()
    {
        prompt = PlayerControlPromptUI.Show(ControlType.Interact, transform.position + Vector3.up);
    }

    protected override void OnPlayerExit()
    {
        if (prompt != null) prompt.Hide();
    }

    private void Update()
    {
        if (playerIsAbove && PlayerInputHandler.PlayerInput.Interact)
        {
            OutdoorIndoorHandler.Instance.SetIndoorOutdoor(isIndoor: !TunnelUser.Instance.IsInTunnsel);
            TunnelUser.Instance.TrySetTunnelState(!TunnelUser.Instance.IsInTunnsel);
        }
    }
}
