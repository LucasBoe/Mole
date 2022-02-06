using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelUser : MonoBehaviour
{
    public bool IsInTunnel = false;
    [SerializeField] CapsuleCollider2D defaultCollider, inTunnelCollider;
    SpriteRenderer[] playerSpriteRenderers;
    [SerializeField] string defaultLayer, inTunnselLayer;

    private void Awake()
    {
        playerSpriteRenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        TunnelState.OnSetTunnelState += SetTunnelState;
    }
    private void SetTunnelState(bool inTunnel)
    {
        IsInTunnel = inTunnel;
        defaultCollider.enabled = !inTunnel;
        inTunnelCollider.enabled = inTunnel;

        foreach (SpriteRenderer spriteRenderer in playerSpriteRenderers)
            spriteRenderer.sortingLayerName = inTunnel ? inTunnselLayer : defaultLayer;
    }
}
