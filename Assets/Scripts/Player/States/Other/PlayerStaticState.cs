using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IStaticTargetProvider
{
    bool IsActive { get; }

    Transform GetTransform();
    bool ProvidesCustomActionCallback();
    InputAction GetCustomExitAction();
}

public static class IStaticTargetProviderExtention
{
    public static IStaticTargetProvider GetClosest(this IStaticTargetProvider[] staticTargetProviders, Vector2 position)
    {
        Transform closest = staticTargetProviders.Select(p => p.GetTransform()).OrderBy(h => Vector2.Distance(h.transform.position, position)).First();
        return staticTargetProviders.Where(p => p.GetTransform() == closest).First();
    }
}

public class PlayerStaticState : PlayerStateBase
{
    protected IStaticTargetProvider Target;
    InputAction exitAction;
    Vector2 posBefore;
    float distance;

    public PlayerStaticState() : base() { }

    public override void Enter()
    {
        base.Enter();

        if (Target.GetTransform() != null)
        {
            posBefore = context.PlayerPos;
            distance = Vector2.Distance(posBefore, Target.GetTransform().position);

            exitAction = Target.ProvidesCustomActionCallback() ?
                Target.GetCustomExitAction() :
                new InputAction() { ActionCallback = DefaultExitCallback, Input = ControlType.Back, Target = Target.GetTransform(), Text = "Exit", Stage = InputActionStage.WorldObject };

            PlayerInputActionRegister.Instance.RegisterInputAction(exitAction);

            SetGravityActive(false);
        }
        else
        {
            SetState(new IdleState());
        }
    }

    public override void Update()
    {
        Transform targetTransform = Target.GetTransform();
        if (targetTransform == null || !Target.IsActive)
            DefaultExitCallback();
        else
        {
            Vector2 pos = Vector2.MoveTowards(context.PlayerPos, targetTransform.position, (distance * Time.deltaTime) / context.Values.SnapToHideablePositionDuration);
            context.Rigidbody.MovePosition(pos);
        }
    }

    public override void Exit()
    {
        base.Exit();
        PlayerInputActionRegister.Instance.UnregisterInputAction(exitAction);
        SetGravityActive(true);
    }

    private void DefaultExitCallback()
    {
        SetState(new IdleState());
    }
}
