using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ParamAnimations/PlayerWalkParamAnimation")]
public class PlayerWalkParamAnimation : ParameterBasedAnimation<WalkState>
{
    [SerializeField] Sprite[] sprint, crouch;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        if (State.IsSprinting)
            return sprint[(int)(Mathf.Floor(Time.time * (sprint.Length + 0.5f)) % sprint.Length)];
        else
            return crouch[(int)(Mathf.Floor((Time.time / 2) * (crouch.Length + 0.5f)) % crouch.Length)];
    }
}

