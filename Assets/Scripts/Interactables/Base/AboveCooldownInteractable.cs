using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AboveCooldownInteractable : CustomBehaviour
{
    [SerializeField] protected bool playerIsAbove = false;
    [SerializeField] bool useCooldown = true;
    protected float enableTime = 0f;
    private Collider2D trigger;

    protected virtual void OnEnable()
    {
        enableTime = Time.time;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        playerIsAbove = true;

        if (CooldownFinished())
            OnPlayerEnter();
        else
            OnEnableWithPlayerAbove();
    }

    protected virtual bool CooldownFinished()
    {
        return !useCooldown ? true : (enableTime + 0.1f) < Time.time;
    }


    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        playerIsAbove = false;

        if (CooldownFinished())
            OnPlayerExit();
    }
    protected void UpdateTrigger()
    {
        Vector2 offset = trigger.offset;
        trigger.enabled = false;
        trigger.offset = Vector2.down;
        trigger.enabled = true;
        trigger.offset = offset;
    }

    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
    protected virtual void OnEnableWithPlayerAbove() { }
}
