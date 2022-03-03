using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHidingTrigger : MonoBehaviour
{
    public PlayerHidingHandler.HiddenAmount HiddenAmount;
    public static System.Action<PlayerHidingTrigger> TriggerEntered, TriggerExited;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            TriggerEntered?.Invoke(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            TriggerExited?.Invoke(this);
    }
}
