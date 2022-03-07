using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SubTree : ActionNode
{
    public BehaviourTree Tree;
    [ReadOnly] public BehaviourTree TreeInstance;
    protected override void OnStart()  { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        return TreeInstance.rootNode.Update();
    }

    public override void Bind(Context context, Blackboard blackboard)
    {
        TreeInstance = Tree.Clone();
        base.Bind(context, blackboard);
        TreeInstance.Bind(context, blackboard);
    }
}
