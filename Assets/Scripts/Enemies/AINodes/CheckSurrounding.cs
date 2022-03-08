using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckSurrounding : Say
{
    protected override void OnStart()
    {
        context.playerDetectionModule.StartChecking();
    }

    protected override void OnStop()
    {
        base.OnStart();
        context.playerDetectionModule.StopChecking();
    }

    protected override State OnUpdate()
    {
        return context.playerDetectionModule.IsChecking ? State.Running : State.Success;
    }
}
