using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableColliderWhileHiding : MonoBehaviour
{
    [SerializeField] private new Collider2D collider2D;

    private void OnEnable()
    {
        HidingState.EnterState += OnStartHiding;
        HidingState.ExitState += OnEndHiding;
    }

    private void OnDisable()
    {
        HidingState.EnterState += OnStartHiding;
        HidingState.ExitState += OnEndHiding;
    }

    private void OnEndHiding()
    {
        collider2D.enabled = true;
    }

    private void OnStartHiding()
    {
        collider2D.enabled = false;
    }

}
