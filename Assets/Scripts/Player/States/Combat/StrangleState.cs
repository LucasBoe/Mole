using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangleState : PlayerCombatState
{
    float strangleProgression = 0;
    public const float strangleDuration = 3;
    PlayerActionProgressionVisualizerUI uiElement;
    PositionInterpolation playerMove;

    public StrangleState(ICombatTarget target) : base(target) { }

    public override void Enter()
    {
        base.Enter();
        strangleProgression = 0;

        if (!target.StartStrangling())
            ExitCombat();
        else
        {
            uiElement = UIHandler.Temporary.Spawn<PlayerActionProgressionVisualizerUI>() as PlayerActionProgressionVisualizerUI;
            playerMove = new PositionInterpolation(context.PlayerPos, target.Position, AnimationCurveHolder.Ease, speed: 10f);
        }
    }

    public override void Update()
    {
        base.Update();

        Log("StrangleState.Update" + context.Input.HoldingUse);

        if (!playerMove.Done) context.Rigidbody.MovePosition(playerMove.Evaluate());

        if (context.Input.HoldingUse)
            strangleProgression += Time.deltaTime;
        else
            SetState(new IdleState());

        uiElement.UpdaValue(strangleProgression / strangleDuration);

        if (strangleProgression >= strangleDuration)
        {
            target.Kill();
            ExitCombat();
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (uiElement != null) uiElement.Hide();
        if (!target.IsNull) target.StopStrangling(context.PlayerPos);
    }
}
