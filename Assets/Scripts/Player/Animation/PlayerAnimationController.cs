using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : PlayerBehaviour
{
    Rigidbody2D playerRigidbody;
    PlayerStateMachine player;
    Animator animator;
    SpriteRenderer spriteRenderer;

    ParamBasedAnimationPair current;

    [SerializeField] StateToAnimationHolder stateToAnimationHolder;
    [SerializeField] GameObject coatGameObject;

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
        foreach (ParamBasedAnimationPair param in stateToAnimationHolder.climbStateParamAnimationPairs)
            param.ParamAnimation.Init(player);
    }

    private void OnStateChange(PlayerStateBase state)
    {
        current = stateToAnimationHolder.GetClipParam(state);

        if (current == null)
        {
            if (!animator.isActiveAndEnabled)
                animator.enabled = true;

            ClipBasedAnimationPair clip = stateToAnimationHolder.GetClip(state);
            if (clip != null)
            {
                animator.Play(clip.AnimationClip.name);
                coatGameObject.SetActive(clip.showCoat);
            }
        }
        else
        {
            current.ParamAnimation.SetState(state);
            coatGameObject.SetActive(current.showCoat);

            if (animator.isActiveAndEnabled)
                animator.enabled = false;
        }
    }

    private void Update()
    {
        if (player.CurrentState == null)
            return;

        animator.SetFloat("speed", playerRigidbody.velocity.magnitude);

        if (current != null)
        {
            ParameterBasedAnimationBase param = current.ParamAnimation;
            spriteRenderer.sprite = param.Update();
            if (param.FlipOverride != ParameterBasedAnimationBase.FlipOverrides.DontUpdate)
            {
                if (param.FlipOverride == ParameterBasedAnimationBase.FlipOverrides.None)
                    spriteRenderer.flipX = playerRigidbody.velocity.x < 0.01f;
                else
                    spriteRenderer.flipX = param.FlipOverride == ParameterBasedAnimationBase.FlipOverrides.Left ? true : false;
            }
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
    public ClipBasedAnimationPair[] moveStateAnimationPairs;
    public ParamBasedAnimationPair[] climbStateParamAnimationPairs;

    internal ClipBasedAnimationPair GetClip(PlayerStateBase state)
    {
        foreach (ClipBasedAnimationPair pair in moveStateAnimationPairs)
        {
            if (pair.MoveState == state.ToString())
                return pair;
        }

        return null;
    }

    internal ParamBasedAnimationPair GetClipParam(PlayerStateBase state)
    {
        foreach (ParamBasedAnimationPair pair in climbStateParamAnimationPairs)
        {
            if (pair.ClimbState == state.ToString())
                return pair;
        }

        return null;
    }
}

[System.Serializable]
public class ClipBasedAnimationPair
{
    public string MoveState;
    public AnimationClip AnimationClip;
    public bool showCoat = true;
}

[System.Serializable]
public class ParamBasedAnimationPair
{
    public string ClimbState;
    public ParameterBasedAnimationBase ParamAnimation;
    public bool showCoat = true;
}
