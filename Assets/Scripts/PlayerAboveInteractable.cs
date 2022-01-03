using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAboveInteractable : MonoBehaviour
{
    [SerializeField] protected bool playerIsAbove = false;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        playerIsAbove = true;
        OnPlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        playerIsAbove = false;
        OnPlayerExit();
    }

    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
}
