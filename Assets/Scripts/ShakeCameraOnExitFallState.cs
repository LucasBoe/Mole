using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraOnExitFallState : MonoBehaviour
{
    private void OnEnable()
    {
        FallState.ExitFallState += OnFallStateExit;
    }
    private void OnDisable()
    {
        FallState.ExitFallState -= OnFallStateExit;
    }

    private void OnFallStateExit(float fallDuration)
    {
        if (fallDuration > 1f)
            CameraShaker.Instance.Shake(transform.position, strength: 3f, frequency: 0.1f);
    }
}
