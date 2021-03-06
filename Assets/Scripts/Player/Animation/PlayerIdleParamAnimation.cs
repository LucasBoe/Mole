using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ParamAnimations/PlayerIdleParamAnimation")]
public class PlayerIdleParamAnimation : ParameterBasedAnimation<IdleState>
{
    [SerializeField] Sprite atWallSprite;
    [SerializeField] Sprite[] idleStand, idleCrouch;
    private Sprite[] idle => State.IsCrouching ? idleCrouch : idleStand;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        if (State.IsAtWall)
            return atWallSprite;

        return idle[(int)(Mathf.Floor(Time.time * (idle.Length + 0.5f)) % idle.Length)];
    }
}

