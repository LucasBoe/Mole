using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableStaticTargetProvider : IStaticTargetProvider
{
    void Move(Vector2 velocity);
}

public class HidingCanMoveState : HidingState
{
    private new IMoveableStaticTargetProvider target;
    public override float HiddenValue => isMoving ? 0.5f : 0;
    private bool isMoving;
    public HidingCanMoveState(IMoveableStaticTargetProvider targetProvider) : base(targetProvider)
    {
        target = targetProvider;
    }
    protected override void SetHiding(bool isHiding)
    {
        if (isHiding)
            SetHidingMode(this);
        else
            SetHidingMode(PlayerHidingHandler.HidingMode.Auto);
    }

    public override void Update()
    {
        base.Update();

        isMoving = context.TriesMoveLeftRight;

        if (isMoving)
        {
            Vector2 movement = Vector2.right * context.Input.Axis.x;
            target.Move(movement);
        }
    }
}
