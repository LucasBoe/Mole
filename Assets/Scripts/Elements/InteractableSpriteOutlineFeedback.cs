using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractableSpriteOutlineFeedback : SpriteOutlineRenderer
{
    private void OnEnable()
    {
        Interactable interactable = GetComponent<Interactable>();
        interactable.PlayerEnterEvent += OnPlayerEnter;
        interactable.PlayerExitEvent += OnPlayerExit;
    }
    private void OnDisable()
    {
        Interactable interactable = GetComponent<Interactable>();
        interactable.PlayerEnterEvent -= OnPlayerEnter;
        interactable.PlayerExitEvent -= OnPlayerExit;
    }

    private void OnPlayerEnter()
    {
        SetOutline(OutlineColor);
    }
    private void OnPlayerExit()
    {
        SetOutline(new Color(0, 0, 0, 0));
    }
}
