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
    }

    [SerializeField] CapsuleCollider2D defaultCollider, inTunnelCollider;

    private bool isActive = true;
    private ColliderMode mode = ColliderMode.Default;

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
        defaultCollider.enabled = isActive && mode == ColliderMode.Default;
        inTunnelCollider.enabled = isActive && mode == ColliderMode.Tunnel;
    }
}
