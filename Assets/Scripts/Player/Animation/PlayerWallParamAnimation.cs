using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallParamAnimation : ParameterBasedAnimation<GutterClimbState>
{
    [SerializeField] Sprite baseSprite;
    [SerializeField] Sprite[] climbing;
    [SerializeField] FloatSpritePair[] ledgeClimbing;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        FlipOverride = State.IsLeft ? FlipOverrides.Left : FlipOverrides.Right;

        if (State.DistanceFromTop > 0)
        {
            foreach (FloatSpritePair pair in ledgeClimbing)
            {
                if (State.DistanceFromTop < pair.Value)
                    return pair.Sprite;
            }

            return ledgeClimbing[0].Sprite;
        }
        else
        {
            if (State.IsMoving)
                return climbing[(int)(Mathf.Floor(Time.time * (climbing.Length + 0.5f)) % climbing.Length)];

            return baseSprite;
        }
    }
}

