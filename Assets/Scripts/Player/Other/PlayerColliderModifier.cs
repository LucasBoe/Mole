using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderModifier : SingletonBehaviour<PlayerColliderModifier>
{
    public enum ColliderMode
    {
        Default,
        Tunnel,
        Hanging,
    }

    [SerializeField] Collider2D defaultCollider, inTunnelCollider, hangingCollider;

    private bool isActive = true;
    private ColliderMode mode = ColliderMode.Default;
    public ColliderMode Mode => mode;

    internal void SetActive(bool active)
    {
        isActive = active;
        UpdateColliders();
    }

    internal void SetMode(ColliderMode colliderMode)
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
}
