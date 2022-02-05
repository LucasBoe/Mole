using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHidingHandler : SingletonBehaviour<PlayerHidingHandler>
{
    public float PlayerHiddenValue => hiddenByState ? 0 : playerHiddenValue;
    private float playerHiddenValue;

    private bool hiddenByState = false;

    private void Start()
    {
        PlayerBrightnessSampler.OnSampleNewPlayerBrightness += UpdateHiddenValueFromPlayerSample;
        PlayerStateMachine.Instance.OnStateChange += OnPlayerEnterState;
    }

    private void OnPlayerEnterState(PlayerStateBase state)
    {
        switch (state.ToString())
        {
            case "InWindowState":
            case "TunnelState":
            case "HidingState":
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
