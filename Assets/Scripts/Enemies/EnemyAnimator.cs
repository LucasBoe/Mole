using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : EnemyModule<EnemyAnimator>
{
    SpriteRenderer spriteRenderer;
    Animator animator;
    EnemyViewconeModule viewconeModule;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        viewconeModule = GetModule<EnemyViewconeModule>();

        viewconeModule.OnStartLookingAround += delegate () { animator.Play("Enemy_LookAround"); };
        GetModule<EnemyMoveModule>().OnStartMovingToPosition += delegate () { animator.Play("Enemy_Walk"); };
        GetModule<EnemyStatemachineModule>().OnStartWaiting += delegate () { animator.Play("Enemy_Idle"); };
        GetModule<EnemyAIModule>().OnStartBeeingStrangled += delegate () { animator.Play("Enemy_Strangled"); };
    }

    private void Update()
    {
        transform.localScale = new Vector3(viewconeModule.IsLookingLeft ? -1 : 1, 1, 1);
    }
}
