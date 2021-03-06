using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class EnemyAnimator : EnemyModule<EnemyAnimator>
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform flipParent;

    private void Start()
    {
        GetModule<EnemyMemoryModule>().ChangedForward += OnChangedForward;
        GetComponent<BehaviourTreeRunner>().Context.EnteredState += OnStateEnter;
    }

    private void OnStateEnter(Node node)
    {
        if (node.animationOverride != null)
        {
            Log("Try play animation: " + node.animationOverride.name);
            animator.Play(node.animationOverride.name);
        }
    }

    private void OnChangedForward(Direction2D forward)
    {
        bool flip = forward == Direction2D.Left;
        spriteRenderer.flipX = flip;
        flipParent.transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
    }

    private void OnDestroy()
    {
        GetModule<EnemyMemoryModule>().ChangedForward -= OnChangedForward;
        GetComponent<BehaviourTreeRunner>().Context.EnteredState -= OnStateEnter;
    }
}

