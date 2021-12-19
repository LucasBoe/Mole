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
        GetModule<EnemyStatemachineModule>().OnStartWaiting += delegate () { animator.Play("Enemy_idle"); };
    }

    private void Update()
    {
        transform.localScale = new Vector3((viewconeModule.transform.rotation.eulerAngles.z > 90 || viewconeModule.transform.rotation.eulerAngles.z < -90) ? -1 : 1, 1, 1);
    }
}
