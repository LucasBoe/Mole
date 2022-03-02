using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerKickParamAnimation : ParameterBasedAnimation<KickState>
{
    [SerializeField] Sprite before, after;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        FlipOverride = State.DirectionIsRight ? FlipOverrides.Right : FlipOverrides.Left;
        return State.Finished ? after : before;
    }
}
