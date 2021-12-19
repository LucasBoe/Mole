using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingHandler : SingletonBehaviour<HidingHandler>
{
    public float PlayerHiddenValue => playerHiddenValue;
    private float playerHiddenValue;

    private void Start()
    {
        PlayerBrightnessSampler.OnSampleNewPlayerBrightness += UpdateHiddenValueFromPlayerSample;
    }

    private void UpdateHiddenValueFromPlayerSample(float sample)
    {
        playerHiddenValue = (sample * 3.5f) - 0.1f;
    }
}
