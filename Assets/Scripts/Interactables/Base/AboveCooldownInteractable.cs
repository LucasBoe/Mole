using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboveCooldownInteractable : MonoBehaviour
{
    [SerializeField] protected bool playerIsAbove = false;
    [SerializeField] bool useCooldown = true;
    protected float enableTime = 0f;

    protected virtual void OnEnable()
    {
        enableTime = Time.time;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
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


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        playerIsAbove = false;

        if (CooldownFinished())
            OnPlayerExit();
    }

    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
    protected virtual void OnEnableWithPlayerAbove() { }
}
