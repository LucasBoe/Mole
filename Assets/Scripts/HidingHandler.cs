using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingHandler : SingletonBehaviour<HidingHandler>
{
    public float PlayerHiddenValue => hiddenByState ? 0 : playerHiddenValue;
    private float playerHiddenValue;

    private bool hiddenByState = false;

    private void Start()
    {
        PlayerBrightnessSampler.OnSampleNewPlayerBrightness += UpdateHiddenValueFromPlayerSample;
        PlayerStateMachine.Instance.OnStateChange += OnPlayerEnterState;
    }

    private void OnPlayerEnterState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.InWindow:
            case PlayerState.Tunnel:
            case PlayerState.Hiding:
                hiddenByState = true;
                break;

            default:
                hiddenByState = false;
                break;
        }
    }

    private void UpdateHiddenValueFromPlayerSample(float sample)
    {
        playerHiddenValue = (sample * 3.5f) - 0.1f;
    }
}
