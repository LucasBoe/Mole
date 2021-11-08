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

    private void OnStateChangePrevious(PlayerMovementState previousBase, PlayerClimbState previousClimb, PlayerMovementState newBase, PlayerClimbState newClimb)
    {
        spriteRenderer.flipX = playerRigidbody.velocity.x < 0.01f;
        animator.SetFloat("speed", playerRigidbody.velocity.magnitude);
        animator.SetTrigger(newBase.ToString());
        animator.SetTrigger(newClimb.ToString());
    }

    private void OnDisable()
    {
        player.OnStateChangePrevious -= OnStateChangePrevious;
    }
}
