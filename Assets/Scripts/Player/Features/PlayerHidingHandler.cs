using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHidingHandler : PlayerSingletonBehaviour<PlayerHidingHandler>
{
    public float PlayerHiddenValue => GetHiddenValue();

    private float GetHiddenValue()
    {
        switch (hidingMode)
        {
            case HidingMode.StateStatic:
                return 0;

            case HidingMode.StateDynamic:
                return hidingState.HiddenValue;
        }

        return playerHiddenValue;
    }

    [SerializeField, ReadOnly] private float playerHiddenValue;
    [SerializeField, ReadOnly] private HidingMode hidingMode = HidingMode.Auto;
    [SerializeField] private HidingState hidingState;

    private bool hiddenByState = false;
    [SerializeField, ReadOnly] private List<LightTrigger> playerTriggers = new List<LightTrigger>();

    public static System.Action<float> ChangedHiddenValue; 

    private void Start()
    {
        PlayerBrightnessSampler.OnSampleNewPlayerBrightness += UpdateHiddenValueFromPlayerSample;
        PlayerStateMachine.Instance.OnStateChange += OnPlayerEnterState;
        LightTrigger.PlayerEnteredTrigger += OnTriggerEntered;
        LightTrigger.PlayerExitedTrigger += OnTriggerExited;
        UpdateHiddenValueFromTriggers(new List<LightTrigger>());
    }

    private void OnTriggerExited(LightTrigger trigger)
    {
        if (playerTriggers.Contains(trigger))
            playerTriggers.Remove(trigger);

        UpdateHiddenValueFromTriggers(playerTriggers);
    }

    private void OnTriggerEntered(LightTrigger trigger)
    {
        if (!playerTriggers.Contains(trigger))
            playerTriggers.Add(trigger);

        UpdateHiddenValueFromTriggers(playerTriggers);
    }

    public void SetHidingMode(HidingMode newHidingMode, HidingState newHidingState = null)
    {
        hidingMode = newHidingMode;
        hidingState = newHidingState;
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

    private void UpdateHiddenValueFromTriggers(List<LightTrigger> playerTriggers)
    {
        if (playerTriggers.Count == 0)
            playerHiddenValue = 0.5f;
        else
        {
            HiddenAmount smallest = playerTriggers.OrderBy(t => t.HiddenAmount).First().HiddenAmount;
            playerHiddenValue = ((int)smallest) / 2f;
        }

        ChangedHiddenValue?.Invoke(playerHiddenValue);
    }

    private void UpdateHiddenValueFromPlayerSample(float sample)
    {
        //playerHiddenValue = (sample * 3.5f) - 0.1f;
    }

    public enum HiddenAmount
    {
        Hidden,
        Neutral,
        Visible,
    }

    public enum HidingMode
    {
        Auto,
        StateStatic,
        StateDynamic
    }
}
