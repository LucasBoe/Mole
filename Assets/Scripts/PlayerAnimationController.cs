using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    PlayerController player;
    Animator animator;
    SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        player = GetComponentInParent<PlayerController>();
        playerRigidbody = GetComponentInParent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player.OnStateChangePrevious += OnStateChangePrevious;
    }

    private void OnStateChangePrevious(PlayerBaseState previousBase, PlayerMoveState previousMove, PlayerClimbState previousClimb, PlayerBaseState newBase, PlayerMoveState newMove, PlayerClimbState newClimb)
    {
        animator.SetBool(previousBase.ToString(), false);

        if (previousMove != PlayerMoveState.None)
            animator.SetBool(previousMove.ToString(), false);

        if (previousClimb != PlayerClimbState.None)
            animator.SetBool(previousClimb.ToString(), false);

        animator.SetBool(newBase.ToString(), true);

        if (newMove != PlayerMoveState.None)
            animator.SetBool(newMove.ToString(), true);

        if (newClimb != PlayerClimbState.None)
            animator.SetBool(newClimb.ToString(), true);
    }

    private void Update()
    {
        if (player.ClimbState != PlayerClimbState.Wall)
            spriteRenderer.flipX = playerRigidbody.velocity.x < 0.01f;
        animator.SetFloat("speed", playerRigidbody.velocity.magnitude);
    }

    private void OnDisable()
    {
        player.OnStateChangePrevious -= OnStateChangePrevious;
    }
}
