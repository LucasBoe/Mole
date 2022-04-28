using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal interface IBalistaTarget
{
    Vector3 Position { get; }
    void SetAimAt(bool isAiming);
    bool NeedsRope { get; }
}
public class BalistaTarget : MonoBehaviour , IBalistaTarget
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite idleSprite, aimAtSprite;

    public Vector3 Position => transform.position;

    [SerializeField] bool needsRope;
    [HideInInspector] public bool NeedsRope => needsRope;

    public void SetAimAt(bool isAiming)
    {
        spriteRenderer.sprite = isAiming ? aimAtSprite : idleSprite;
    }
}