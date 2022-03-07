using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoiseMaker : PlayerBehaviour
{
    private void OnEnable()
    {
        WalkState.Walk += OnWalk;
    }

    private void OnDisable()
    {
        WalkState.Walk -= OnWalk;
    }

    private void OnWalk(bool walk)
    {
        StopAllCoroutines();
        if (walk)
            StartCoroutine(CheckForSprintRoutine());
    }

    private IEnumerator CheckForSprintRoutine()
    {
        while (true)
        {
            if ((PlayerStateMachine.Instance.CurrentState as WalkState).IsSprinting)
                NoiseHandler.Instance.MakeNoise(transform.position, 7f);

            yield return new WaitForSeconds(0.25f);
        }
    }
}
