using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelUser : SingletonBehaviour<TunnelUser>
{
    private float lastTunnelSwitch = 0f;

    public bool IsInTunnsel = false;
    [SerializeField] CapsuleCollider2D defaultCollider, inTunnelCollider;
    [SerializeField] SpriteRenderer playerSpriteRenderer;
    [SerializeField] string defaultLayer, inTunnselLayer;

    public void TrySetTunnelState(bool inTunnel)
    {
        if (Time.time < lastTunnelSwitch + 0.1f)
            return;

        IsInTunnsel = inTunnel;
        inTunnelCollider.enabled = inTunnel;
        defaultCollider.enabled = !inTunnel;
        //playerSpriteRenderer.sortingLayerName = inTunnel ? inTunnselLayer : defaultLayer;

        lastTunnelSwitch = Time.time;

        PlayerStateMachine.Instance.SetState(inTunnel ? PlayerState.Tunnel : PlayerState.Idle);

        Debug.LogWarning($"Player is in tunnel: {inTunnel}");
    }
}
