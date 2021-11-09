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

    private void OnStateChangePrevious(PlayerBaseState previousBase, PlayerClimbState previousClimb, PlayerBaseState newBase, PlayerClimbState newClimb)
    {
        animator.SetBool(previousBase.ToString(), false);
        if (previousClimb != PlayerClimbState.None)
            animator.SetBool(previousClimb.ToString(), false);
        animator.SetBool(newBase.ToString(), true);
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
