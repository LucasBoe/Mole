using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class Say : ActionNode
{
    public string[] toSay;
    protected override void OnStart()
    {
        WorldTextSpawner.Spawn(toSay.GetRandom(), context.transform.position + new Vector3(0, 2, 0));
    }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}
