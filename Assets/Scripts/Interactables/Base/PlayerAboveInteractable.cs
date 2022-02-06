using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAboveInteractable : MonoBehaviour
{
    [SerializeField] protected bool playerIsAbove = false;
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

        if (DidNotJustSpawn())
            OnPlayerEnter();
        else
            OnEnableWithPlayerAbove();
    }

    protected virtual bool DidNotJustSpawn()
    {
        return (enableTime + 0.1f) < Time.time;
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        playerIsAbove = false;

        if (DidNotJustSpawn())
            OnPlayerExit();
    }

    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
    protected virtual void OnEnableWithPlayerAbove() { }
}
