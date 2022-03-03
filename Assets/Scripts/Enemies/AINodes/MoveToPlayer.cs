using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToPlayer : MoveBaseNode
{
    protected override void OnStart()
    {
        MoveTo(context.memory.PlayerPos);
    }
}
