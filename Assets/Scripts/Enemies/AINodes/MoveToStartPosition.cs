using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToStartPosition : MoveBaseNode
{
    protected override void OnStart()
    {
        MoveTo(context.memory.PositionOriginal);
    }
}
