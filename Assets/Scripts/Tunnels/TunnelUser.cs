using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelUser : MonoBehaviour
{
    public bool IsInTunnel = false;
    SpriteRenderer[] playerSpriteRenderers;
    [SerializeField] string defaultLayer, inTunnselLayer;

    private void Awake()
    {
        playerSpriteRenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        PlayerStateMachine.Instance.OnStateChange += CheckForChangeTunnelState;
    }

    private void CheckForChangeTunnelState(PlayerStateBase state)
    {
        Type newState = state.GetType();
        
        if (!IsInTunnel && newState == typeof(TunnelState))
        {
            SetTunnelState(true);
        } else if (IsInTunnel && newState != typeof(TunnelState) && newState != typeof(SpyingState))
        {
            SetTunnelState(false);
        }
    }

    private void SetTunnelState(bool inTunnel)
    {
        IsInTunnel = inTunnel;
        PlayerColliderModifier.Instance.SetMode(inTunnel ? PlayerColliderModifier.ColliderMode.Tunnel : PlayerColliderModifier.ColliderMode.Default);

        foreach (SpriteRenderer spriteRenderer in playerSpriteRenderers)
            spriteRenderer.sortingLayerName = inTunnel ? inTunnselLayer : defaultLayer;
    }
}
