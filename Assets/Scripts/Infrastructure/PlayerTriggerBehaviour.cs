using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            OnPlayerEnter2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            OnPlayerExit2D(collision);
    }

    protected virtual void OnPlayerEnter2D(Collider2D playerCollider) { }
    protected virtual void OnPlayerExit2D(Collider2D playerCollider) { }
}
