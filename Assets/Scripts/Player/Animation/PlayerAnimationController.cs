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
    WallState wallState;

   [SerializeField] StateToAnimationHolder stateToAnimationHolder;
    private void OnEnable()
    {
        player = GetComponentInParent<PlayerController>();
        playerRigidbody = GetComponentInParent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player.OnStateChange += OnStateChange;
    }

    private void Start()
    {
        wallState = (player.climbStateDictionary[PlayerClimbState.Wall] as WallState);
    }

    private void OnStateChange(PlayerBaseState baseState, PlayerMoveState move, PlayerClimbState climb)
    {
        AnimationClip toPlay = stateToAnimationHolder.GetClip(baseState, move, climb);
        if (toPlay != null)
            animator.Play(toPlay.name);
    }

    private void Update()
    {
        bool flip = playerRigidbody.velocity.x < 0.01f;

        if (player.ClimbState == PlayerClimbState.Wall)
        {
            flip = wallState.IsLeft;
        }

        spriteRenderer.flipX = flip;
        animator.SetFloat("speed", playerRigidbody.velocity.magnitude);
    }

    private void OnDisable()
    {
        player.OnStateChange -= OnStateChange;
    }
}

[System.Serializable]
public class StateToAnimationHolder
{
    public MoveStateAnimationPair[] moveStateAnimationPairs;
    public ClimbStateAnimationPair[] climbStateAnimationPairs;

    internal AnimationClip GetClip(PlayerBaseState baseState, PlayerMoveState move, PlayerClimbState climb)
    {
        if (baseState == PlayerBaseState.Climb)
        {
            foreach (ClimbStateAnimationPair pair in climbStateAnimationPairs)
            {
                if (pair.ClimbState == climb)
                    return pair.AnimationClip;
            }
        } else
        {
            foreach (MoveStateAnimationPair pair in moveStateAnimationPairs)
            {
                if (pair.MoveState == move)
                    return pair.AnimationClip;
            }
        }

        return null;
    }
}

[System.Serializable]
public class MoveStateAnimationPair
{
    public PlayerMoveState MoveState;
    public AnimationClip AnimationClip;
}

[System.Serializable]
public class ClimbStateAnimationPair
{
    public PlayerClimbState ClimbState;
    public AnimationClip AnimationClip;
}
