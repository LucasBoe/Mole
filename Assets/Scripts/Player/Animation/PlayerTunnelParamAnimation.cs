using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ParamAnimations/PlayerTunnelParamAnimation")]
public class PlayerTunnelParamAnimation : ParameterBasedAnimation<TunnelState>
{
    [SerializeField] Sprite[] vertical, horizontal;

    private Sprite sprite;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        if (sprite == null)
            sprite = horizontal[0];

        if (State.MoveDir != Vector2Int.zero)
        {
            sprite = GetSpriteForDir(State.MoveDir);
        }

        return sprite;
    }

    private Sprite GetSpriteForDir(Vector2Int moveDir)
    {
        Sprite[] sprites = null;

        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
            sprites = horizontal;
        else
            sprites = vertical;

        return sprites[(int)(Mathf.Floor(Time.time * (sprites.Length + 0.5f)) % sprites.Length)];
    }
}

