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
    }


    public void TrySetTunnelState(bool inTunnel)
    {
        if (Time.time < lastTunnelSwitch + 0.1f)
            return;

        IsInTunnel = inTunnel;
        defaultCollider.enabled = !inTunnel;
        inTunnelCollider.enabled = inTunnel;
        foreach (SpriteRenderer spriteRenderer in playerSpriteRenderers)
        {
            spriteRenderer.sortingLayerName = inTunnel ? inTunnselLayer : defaultLayer;
        }

        lastTunnelSwitch = Time.time;

        PlayerStateMachine.Instance.SetState(inTunnel ? PlayerState.Tunnel : PlayerState.Idle);
        transform.parent.Translate(inTunnel ? Vector2.down : Vector2.up);
    }
}
