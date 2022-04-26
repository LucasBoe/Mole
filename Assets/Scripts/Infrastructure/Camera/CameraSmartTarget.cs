using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmartTarget : SingletonBehaviour<CameraSmartTarget>
{
    public enum SmartTargetMode
    {
        Default,
        Override,
    }

    [SerializeField] SmartTargetMode mode;
    Transform player;
    Vector2 smoothTargetPosition = Vector2.zero;

    Transform overrideTarget;
    private void OnEnable()
    {
        PlayerController.OnPlayerSpawned += RegisterPlayer;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerSpawned -= RegisterPlayer;
    }
    private void RegisterPlayer(Transform player)
    {
        this.player = player;
    }

    public static void SetOverride (Transform newOverride)
    {
        Instance.overrideTarget = newOverride;
        Instance.mode = newOverride == null ? SmartTargetMode.Default : SmartTargetMode.Override;

        if (Instance.mode == SmartTargetMode.Override)
            Instance.smoothTargetPosition = Instance.player.position;
    }

    private void LateUpdate()
    {
        Vector2 target = CalculateTargetPos();
        transform.position = target;
    }
    private Vector2 CalculateTargetPos()
    {
        switch (mode)
        {
            case SmartTargetMode.Override:
                smoothTargetPosition = Vector2.MoveTowards(smoothTargetPosition, overrideTarget.position, Time.deltaTime * 10f);
                return Vector2.Lerp(player.position, smoothTargetPosition, 0.5f);
        }
        return player.position; 
    }
}
