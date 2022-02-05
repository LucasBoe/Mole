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

    ClimbStateParamAnimationPair current;

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
        foreach (ClimbStateParamAnimationPair param in stateToAnimationHolder.climbStateParamAnimationPairs)
            param.ParamAnimation.Init(player);
    }

    private void OnStateChange(PlayerStateBase state)
    {
        current = stateToAnimationHolder.GetClipParam(state);

        if (current == null)
        {
            if (!animator.isActiveAndEnabled)
                animator.enabled = true;

            MoveStateAnimationPair clip = stateToAnimationHolder.GetClip(state);
            if (clip != null)
            {
                animator.Play(clip.AnimationClip.name);
                coatGameObject.SetActive(clip.showCoat);
            }
        }
        else
        {
            coatGameObject.SetActive(current.showCoat);

            if (animator.isActiveAndEnabled)
                animator.enabled = false;
        }
    }

    private void Update()
    {
        if (player.CurrentState == null)
            return;

        if (player.CurrentState.GetType() != typeof(WallStretchState))
        {
            bool flip = playerRigidbody.velocity.x < 0.01f;

            if (player.CurrentState.GetType() == typeof(WallState))
            {
                flip = (player.CurrentState as WallState).IsLeft;
            }

            spriteRenderer.flipX = flip;
        }

        animator.SetFloat("speed", playerRigidbody.velocity.magnitude);

        if (current != null)
        {
            spriteRenderer.sprite = current.ParamAnimation.Update();
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

    internal MoveStateAnimationPair GetClip(PlayerStateBase state)
    {
        foreach (MoveStateAnimationPair pair in moveStateAnimationPairs)
        {
            if (pair.MoveState == state.ToString())
                return pair;
        }

        return null;
    }

    internal ClimbStateParamAnimationPair GetClipParam(PlayerStateBase state)
    {
        foreach (ClimbStateParamAnimationPair pair in climbStateParamAnimationPairs)
        {
            if (pair.ClimbState == state.ToString())
                return pair;
        }

        return null;
    }
}

[System.Serializable]
public class MoveStateAnimationPair
{
    public string MoveState;
    public AnimationClip AnimationClip;
    public bool showCoat = true;
}

[System.Serializable]
public class ClimbStateParamAnimationPair
{
    public string ClimbState;
    public ParameterBasedAnimationBase ParamAnimation;
    public bool showCoat = true;
}
