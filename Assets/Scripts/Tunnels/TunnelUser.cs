using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelUser : SingletonBehaviour<TunnelUser>
{
    private float lastTunnelSwitch = 0f;

    public bool IsInTunnel = false;
    [SerializeField] CapsuleCollider2D defaultCollider, inTunnelCollider;
    SpriteRenderer[] playerSpriteRenderers;
    [SerializeField] string defaultLayer, inTunnselLayer;

    protected override void Awake()
    {
        base.Awake();
        playerSpriteRenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        LayerHandler.OnChangeLayer += OnChangeLayer; 
    }
    private void OnChangeLayer(Layers before, LayerDataPackage target)
    {
        if (before == Layers.Tunnels)
            SetTunnelState(false);
        else if (target.Layer == Layers.Tunnels)
            SetTunnelState(true);
    }
    public void SetTunnelState(bool inTunnel)
    {
        IsInTunnel = true;
        defaultCollider.enabled = !inTunnel;
        inTunnelCollider.enabled = inTunnel;

        foreach (SpriteRenderer spriteRenderer in playerSpriteRenderers)
            spriteRenderer.sortingLayerName = inTunnel ? inTunnselLayer : defaultLayer;

        //TODO: Remove this variable after entering and looking trough are different actions
        lastTunnelSwitch = Time.time;

        //PlayerStateMachine.Instance.SetState(inTunnel ? PlayerState.Tunnel : new IdleState());
        transform.parent.Translate(inTunnel ? Vector2.down : Vector2.up);
    }
}
