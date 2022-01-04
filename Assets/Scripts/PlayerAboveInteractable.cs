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

        if ((enableTime + 0.1f) < Time.time)
            OnPlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        playerIsAbove = false;

        if ((enableTime + 0.1f) < Time.time)
            OnPlayerExit();
    }

    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
}
