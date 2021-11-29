using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Rigidbody2D playerRigidbody;
    PlayerStateMachine player;
    Animator animator;
    SpriteRenderer spriteRenderer;
    WallState wallState;

    ParameterBasedAnimationBase current;

    [SerializeField] StateToAnimationHolder stateToAnimationHolder;
    private void OnEnable()
    {
        player = GetComponentInParent<PlayerStateMachine>();
        playerRigidbody = GetComponentInParent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player.OnStateChange += OnStateChange;
    }

    private void Start()
    {
        wallState = (player.stateDictionary[PlayerState.Wall] as WallState);

        foreach (ClimbStateParamAnimationPair param in stateToAnimationHolder.climbStateParamAnimationPairs)
            param.ParamAnimation.Init(player);
    }

    private void OnStateChange(PlayerState climb)
    {
        current = stateToAnimationHolder.GetClipParam(climb);

        if (current == null)
        {
            if (!animator.isActiveAndEnabled)
                animator.enabled = true;

            AnimationClip toPlay = stateToAnimationHolder.GetClip(climb);
            if (toPlay != null)
                animator.Play(toPlay.name);
        }
        else
        {
            if (animator.isActiveAndEnabled)
                animator.enabled = false;
        }
    }

    private void Update()
    {
        if (player.CurrentState != PlayerState.WallStretch)
        {
            bool flip = playerRigidbody.velocity.x < 0.01f;

            if (player.CurrentState == PlayerState.Wall)
            {
                flip = wallState.IsLeft;
            }

            spriteRenderer.flipX = flip;
        }

        animator.SetFloat("speed", playerRigidbody.velocity.magnitude);

        if (current != null)
        {
            spriteRenderer.sprite = current.Update();
        }
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
    public ClimbStateParamAnimationPair[] climbStateParamAnimationPairs;

    internal AnimationClip GetClip(PlayerState state)
    {

        foreach (MoveStateAnimationPair pair in moveStateAnimationPairs)
        {
            if (pair.MoveState == state)
                return pair.AnimationClip;
        }

        return null;
    }

    internal ParameterBasedAnimationBase GetClipParam(PlayerState state)
    {
        foreach (ClimbStateParamAnimationPair pair in climbStateParamAnimationPairs)
        {
            if (pair.ClimbState == state)
                return pair.ParamAnimation;
        }

        return null;
    }
}

[System.Serializable]
public class MoveStateAnimationPair
{
    public PlayerState MoveState;
    public AnimationClip AnimationClip;
}

[System.Serializable]
public class ClimbStateParamAnimationPair
{
    public PlayerState ClimbState;
    public ParameterBasedAnimationBase ParamAnimation;
}
