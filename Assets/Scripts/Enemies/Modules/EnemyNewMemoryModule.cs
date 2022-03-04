using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNewMemoryModule : EnemyModule<EnemyNewMemoryModule>
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
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
            Debug.Log($"canSeePlayer = { canSeePlayer }");
        }
    }
    public bool IsAlerted { get; internal set; }

    public System.Action<Direction2D> ChangedForward;

    [SerializeField, ReadOnly] private Rigidbody2D playerBody;
    public Rigidbody2D Player { set { playerBody = value; } }

    public System.Action<EnemyNewMemoryModule> CheckedForPlayerPos;

    protected override void Awake()
    {
        base.Awake();
        forward = spriteRenderer.flipX ? Direction2D.Left : Direction2D.Right;
    }
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
}