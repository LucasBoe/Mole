using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRopeSlider : PlayerSingletonBehaviour<PlayerRopeSlider>
{
    InputAction startSlidingAction;
    ISlideable currentSlideable;

    [SerializeField] private Rigidbody2D sliderBody;

    private void OnEnable()
    {
        startSlidingAction = new InputAction() { Input = ControlType.Use, Text = "Slide", Target = transform, Stage = InputActionStage.WorldObject, ActionCallback = SetStateToSlide };
    }

    internal void SetSliderActive(bool active)
    {
        sliderBody.gameObject.SetActive(active);
    }

    private void SetStateToSlide()
    {
        PlayerStateMachine.Instance.SetState(new SlidingState(currentSlideable));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ISlideable slideable = collision.GetComponent<ISlideable>();

        if (slideable != null)
        {
            currentSlideable = slideable;
            PlayerInputActionRegister.Instance.RegisterInputAction(startSlidingAction);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ISlideable slideable = collision.GetComponent<ISlideable>();

        if (slideable == currentSlideable)
        {
            currentSlideable = null;
            PlayerInputActionRegister.Instance.UnregisterInputAction(startSlidingAction);
        }
    }
}
