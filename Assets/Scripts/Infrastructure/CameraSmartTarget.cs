using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmartTarget : MonoBehaviour
{
    public enum SmartTargetMode
    {
        Default,
        Aim,
    }

    SmartTargetMode mode;
    Transform player;
    private void OnEnable()
    {
        PlayerController.OnPlayerSpawned += RegisterPlayer;
        Crosshair.ChangedCrosshairMode += OnChangedCrosshairMode;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerSpawned -= RegisterPlayer;
        Crosshair.ChangedCrosshairMode -= OnChangedCrosshairMode;
    }
    private void RegisterPlayer(Transform player)
    {
        this.player = player;
    }

    private void LateUpdate()
    {
        Vector2 target = CalculateTargetPos();
        transform.position = target;
    }
    private void OnChangedCrosshairMode(Crosshair.Mode crosshairMode)
    {
        mode = crosshairMode == Crosshair.Mode.Active ? SmartTargetMode.Aim : SmartTargetMode.Default;
    }

    private Vector2 CalculateTargetPos()
    {
        switch (mode)
        {
            case SmartTargetMode.Aim:
                return Vector2.MoveTowards(player.position, new Vector2(Mathf.Lerp(PlayerInputHandler.PlayerInput.VirtualCursorToWorldPos.x, player.position.x, 0.25f), player.position.y), 8f);

        }

        return player.position; 
    }
}
