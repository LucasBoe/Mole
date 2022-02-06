using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System.Linq;

public interface IStaticTargetProvider
{
    Transform GetTransform();
    bool ProvidesCustomActionCallback();
    InputAction GetCustomExitAction();
}

public interface ISpyableTargetProvider : IStaticTargetProvider
{
    Layers GetLayerToSpy();
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
    protected IStaticTargetProvider target;
    InputAction exitAction;
    Vector2 posBefore;
    float distance;

    public PlayerStaticState() : base() { }

    public override void Enter()
    {
        base.Enter();

        if (target.GetTransform() != null)
        {
            posBefore = context.PlayerPos;
            distance = Vector2.Distance(posBefore, target.GetTransform().position);

            exitAction = target.ProvidesCustomActionCallback() ?
                target.GetCustomExitAction() :
                new InputAction() { ActionCallback = DefaultExitCallback, Input = ControlType.Back, Target = target.GetTransform(), Text = "Exit", Stage = InputActionStage.WorldObject };

            PlayerInputActionRegister.Instance.RegisterInputAction(exitAction);

            SetCollisionActive(false);
            SetGravityActive(false);
        }
        else
        {
            SetState(new IdleState());
        }
    }

    public override void Update()
    {
        Transform targetTransform = target.GetTransform();
        if (targetTransform == null)
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
        SetCollisionActive(true);
        SetGravityActive(true);
    }

    private void DefaultExitCallback()
    {

        SetState(new IdleState());
    }
}

public class HidingState : PlayerStaticState
{
    public HidingState(IStaticTargetProvider targetProvider) : base()
    {
        target = targetProvider;
    }
}

public class SpyingState : PlayerStaticState
{
    private Layers sourceLayer;
    private Layers spyLayer;
    InputAction exitSpyStateAction;
    InputAction enterLayerAction;

    public SpyingState(PlayerStateBase stateBefore, Layers sourceLayer, Layers spyLayer, InputAction enterAction)
    {
        Debug.LogWarning("spy from " + sourceLayer + " to " + spyLayer);

        this.sourceLayer = sourceLayer;
        this.spyLayer = spyLayer;

        exitSpyStateAction = new InputAction() { ActionCallback = () => { LayerHandler.Instance.SwitchLayer(sourceLayer); SetState(stateBefore); }, Input = ControlType.Back, Stage = InputActionStage.ModeSpecific, Target = PlayerController.Instance.transform, Text = "Stop Spying" };
        enterLayerAction = enterAction;
    }

    public override void Enter()
    {
        SetCollisionActive(false);
        SetGravityActive(false);

        LayerHandler.Instance.SwitchLayer(spyLayer, spy: true);
        PlayerInputActionRegister.Instance.RegisterInputAction(exitSpyStateAction);
        PlayerInputActionRegister.Instance.RegisterInputAction(enterLayerAction);
    }

    public override void Update() { }

    public override void Exit()
    {
        SetCollisionActive(true);
        SetGravityActive(true);

        PlayerInputActionRegister.Instance.UnregisterInputAction(exitSpyStateAction);
        PlayerInputActionRegister.Instance.UnregisterInputAction(enterLayerAction);
    }
}
