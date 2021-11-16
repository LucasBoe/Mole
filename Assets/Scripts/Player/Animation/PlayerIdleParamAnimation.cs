using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleParamAnimation : ParameterBasedAnimationBase
{
    private IdleState idleState;

    [SerializeField] Sprite atWallSprite;
    [SerializeField] Sprite[] climbing;


    public override void Init(PlayerController playerController)
    {
        idleState = playerController.stateDictionary[PlayerState.Idle] as IdleState;
    }

    public override Sprite Update()
    {
        if (idleState.IsAtWall)
            return atWallSprite;

        return climbing[(int)(Mathf.Floor(Time.time * (climbing.Length + 0.5f)) % climbing.Length)];
    }
}

