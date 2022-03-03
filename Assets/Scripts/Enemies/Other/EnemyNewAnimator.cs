using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNewAnimator : EnemyModule<EnemyNewAnimator>
{
    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        GetModule<EnemyNewMemoryModule>().ChangedForward += OnChangedForward;
    }

    private void OnChangedForward(Direction2D forward)
    {
        spriteRenderer.flipX = forward == Direction2D.Left;
    }

    private void OnDestroy()
    {
        GetModule<EnemyNewMemoryModule>().ChangedForward -= OnChangedForward;
    }
}

