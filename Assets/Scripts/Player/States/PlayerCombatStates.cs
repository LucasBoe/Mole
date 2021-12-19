using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatState : PlayerStateBase
{
    protected ICombatTarget target;
    public PlayerCombatState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        target = context.CombatTarget;
    }

    public override void Update()
    {
        if (target.IsNull)
            ExitCombat();
    }

    protected void ExitCombat()
    {
        SetState(PlayerState.Idle);
    }
}

public class CombatStrangleState : PlayerCombatState
{
    float strangleProgression = 0;
    float strangleDuration = 1;

    PlayerActionProgressionVisualizerUI uiElement;

    public CombatStrangleState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        base.Enter();
        strangleProgression = 0;
        target.StartStrangling();
        uiElement = UIHandler.Temporary.Spawn<PlayerActionProgressionVisualizerUI>() as PlayerActionProgressionVisualizerUI;
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(context.PlayerPos, target.StranglePosition) > 0.1f)
            context.Rigidbody.MovePosition(Vector2.MoveTowards(context.PlayerPos, target.StranglePosition, Time.deltaTime * 25f));

        if (context.Input.HoldingUse)
            strangleProgression += Time.deltaTime;
        else
            SetState(PlayerState.Idle);

        uiElement.UpdaValue(strangleProgression);

        if (strangleProgression >= strangleDuration)
        {
            target.Kill();
            ExitCombat();
        }
    }

    public override void Exit()
    {
        base.Exit();

        uiElement.Hide();

        if (!target.IsNull)
            target.StopStrangling(context.PlayerPos);
    }
}
