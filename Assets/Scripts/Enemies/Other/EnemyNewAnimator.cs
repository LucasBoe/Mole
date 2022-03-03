using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class EnemyNewAnimator : EnemyModule<EnemyNewAnimator>
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        GetModule<EnemyNewMemoryModule>().ChangedForward += OnChangedForward;
        GetComponent<BehaviourTreeRunner>().Context.EnteredState += OnStateEnter;
    }

    private void OnStateEnter(Node node)
    {
        Debug.Log($"node = { node.name }");

        if (node.animationOverride != null)
            animator.Play(node.animationOverride.name);
    }

    private void OnChangedForward(Direction2D forward)
    {
        spriteRenderer.flipX = forward == Direction2D.Left;
    }

    private void OnDestroy()
    {
        GetModule<EnemyNewMemoryModule>().ChangedForward -= OnChangedForward;
        GetComponent<BehaviourTreeRunner>().Context.EnteredState -= OnStateEnter;
    }
}

