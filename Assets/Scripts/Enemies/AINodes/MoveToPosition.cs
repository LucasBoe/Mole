using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToPosition : MoveBaseNode
{
    public EnemyVariableReference targetPosition;
    protected override void OnStart()
    {
        MoveTo(context.variable.Read(targetPosition));
    }
}
