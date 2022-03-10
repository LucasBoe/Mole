using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToClosestLamp : MoveBaseNode
{
    protected override void OnStart()
    {
        MoveTo(context.closestPotentialLamp.transform.position);
    }
}
