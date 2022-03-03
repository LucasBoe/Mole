using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : EnemyModule<EnemyAnimator>
{
    SpriteRenderer spriteRenderer;
    Animator animator;
    EnemyViewconeModule viewconeModule;
    string amimationName;

    bool block;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        viewconeModule = GetModule<EnemyViewconeModule>();

        viewconeModule.OnStartLookingAround += delegate () { PlayAnimation("Enemy_LookAround"); };
        GetModule<EnemyMoveModule>().OnStartMovingToPosition += delegate () { PlayAnimation("Enemy_Walk"); };
        GetModule<EnemyStatemachineModule>().OnStartWaiting += delegate () { PlayAnimation("Enemy_Idle"); };
        GetModule<EnemyAIModule>().OnStartBeeingStrangled += delegate () { PlayAnimation("Enemy_Strangled"); };
        GetModule<EnemyGroundCheckModule>().LeftGround += delegate () { PlayAnimation("Enemy_Fall"); block = true; };
        GetModule<EnemyGroundCheckModule>().EnteredGround += delegate () { block = false; };
    }

    private void PlayAnimation(string layerName)
    {
        if (block)
            return;

        amimationName = layerName;
        animator.Play(layerName);
    }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(amimationName))
            PlayAnimation(amimationName);
    }

    private void Update()
    {
        transform.localScale = new Vector3(viewconeModule.IsLookingLeft ? -1 : 1, 1, 1);
    }
}
