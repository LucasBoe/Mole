using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryModule : EnemyModule<EnemyMemoryModule>
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField, ReadOnly] private Vector2 playerPos;

    [SerializeField, ReadOnly] private bool canSeePlayer = false;
    public bool CanSeePlayer
    {
        get
        {
            return canSeePlayer;
        }

        set
        {
            canSeePlayer = value;
        }
    }

    [ReadOnly] public bool ReactedToAlert = false;
    [SerializeField, ReadOnly] private bool isAlterted;
    public bool IsAlerted
    {
        get => isAlterted;
        set
        {
            bool before = isAlterted;
            isAlterted = value;
            if (isAlterted && !before)
            {
                ReactedToAlert = false;
                AlertedEnter?.Invoke();
            }
            else if (!isAlterted && before)
                AlertedExit?.Invoke();

        }
    }

    public System.Action<Direction2D> ChangedForward;

    [SerializeField, ReadOnly] private Rigidbody2D playerBody;
    public Rigidbody2D Player { set { playerBody = value; } }

    public System.Action<EnemyMemoryModule> CheckedForPlayerPos;
    public System.Action AlertedEnter;
    public System.Action AlertedExit;

    public Vector2 PlayerPos
    {
        get
        {
            CheckedForPlayerPos?.Invoke(this);
            return CanSeePlayer ? playerBody.position : playerPos;
        }
        set
        {
            playerPos = value;
        }
    }
    [SerializeField, ReadOnly] private Direction2D forwardOriginal;
    public Direction2D ForwardOriginal => forwardOriginal;

    [SerializeField, ReadOnly] private Direction2D forward;
    public Direction2D Forward
    {
        get => forward;
        set
        {
            forward = value;
            ChangedForward?.Invoke(forward);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        forward = spriteRenderer.flipX ? Direction2D.Left : Direction2D.Right;
        forwardOriginal = forward;
    }

    internal void Alert(Vector2 noiseLocation)
    {
        Alert();
        PlayerPos = noiseLocation;
    }

    internal void Alert()
    {
        Log("Alert!");
        IsAlerted = true;
    }
}