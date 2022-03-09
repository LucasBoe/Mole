using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
namespace TheKiwiCoder
{
    public class MoveBaseNode : ActionNode
    {
        protected Vector2 target;

        protected override void OnStart() { }
        protected override void OnStop()
        {
            context.moveModule.StopMovingTo(target);
        }

        protected override State OnUpdate()
        {
            return context.moveModule.isMoving ? State.Running : State.Success;
        }

        protected void MoveTo(Vector2 target)
        {
            this.target = target;
            context.memory.Forward = Direction2DUtil.FromPositions(context.transform.position, target);
            context.moveModule.MoveTo(target);
        }
    }
}
