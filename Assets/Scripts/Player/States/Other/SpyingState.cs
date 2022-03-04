using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpyableTargetProvider : IStaticTargetProvider
{
    Layers GetLayerToSpy();
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
