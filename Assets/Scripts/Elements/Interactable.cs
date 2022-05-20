using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : AboveInputActionProvider
{
    [SerializeField] protected Transform interactionOrigin;

    public System.Action PlayerEnterEvent, PlayerExitEvent;

    protected override void OnPlayerEnter()
    {
        PlayerEnterEvent?.Invoke();
        base.OnPlayerEnter();
    }

    protected override void OnPlayerExit()
    {
        PlayerExitEvent?.Invoke();
        base.OnPlayerExit();
    }
}
