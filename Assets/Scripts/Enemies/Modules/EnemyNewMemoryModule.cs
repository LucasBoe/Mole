using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNewMemoryModule : EnemyModule<EnemyNewMemoryModule>
{
    [SerializeField, ReadOnly] private Vector2 playerPos;
    public bool CanSeePlayer { get; internal set; }
    public bool IsAlerted { get; internal set; }

    public System.Action<Direction2D> ChangedForward;

    private Rigidbody2D playerBody;
    public Rigidbody2D Player { set { playerBody = value; } }

    public System.Action<EnemyNewMemoryModule> CheckedForPlayerPos;

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