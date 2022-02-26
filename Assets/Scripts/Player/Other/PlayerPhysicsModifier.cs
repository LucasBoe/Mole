using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicsModifier : SingletonBehaviour<PlayerPhysicsModifier>
{
    public enum ColliderMode
    {
        Default,
        Tunnel,
        Hanging,
    }

    [SerializeField] Rigidbody2D PlayerRigidbody2D;
    [SerializeField] Collider2D defaultCollider, inTunnelCollider, hangingCollider;

    private bool isActive = true;
    private ColliderMode mode = ColliderMode.Default;
    public ColliderMode Mode => mode;

    internal void SetCollisionActive(bool active)
    {
        isActive = active;
        UpdateColliders();
    }

    internal void SetColliderMode(ColliderMode colliderMode)
    {
        mode = colliderMode;
        UpdateColliders();
    }

    private void UpdateColliders()
    {
        defaultCollider.enabled = isActive && (mode == ColliderMode.Default || mode == ColliderMode.Hanging);
        inTunnelCollider.enabled = isActive && mode == ColliderMode.Tunnel;
        hangingCollider.enabled = isActive && mode == ColliderMode.Hanging;
    }

    internal Collider2D GetActiveCollider()
    {
        return mode == ColliderMode.Tunnel ? inTunnelCollider : defaultCollider;
    }

    internal void SetGracityActive(bool active)
    {
        PlayerRigidbody2D.gravityScale = active ? 2 : 0;
    }

    internal void SetPlayerDragActive(bool active)
    {
        PlayerRigidbody2D.drag = active ? 10 : 0;
    }
}
