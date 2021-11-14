using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public bool Right;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            UpdateIndoorOutdoor(collision.transform.position, false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            UpdateIndoorOutdoor(collision.transform.position, true);
    }
    private void UpdateIndoorOutdoor(Vector3 position, bool leaving)
    {
        bool indoor = position.x < transform.position.x;
        OutdoorIndoorHandler.Instance.SetIndoorOutdoor(indoor == leaving);
    }


    private void OnDrawGizmos()
    {
        Vector2 pos = (Vector2)transform.position + (Right ? Vector2.left : Vector2.right) * 0.5f;
        Gizmos.DrawLine(pos + Vector2.left * 0.5f, pos + Vector2.right * 0.5f);
        Gizmos.DrawLine(pos + Vector2.up * 0.5f, pos + (Right ? Vector2.left : Vector2.right) * 0.5f);
        Gizmos.DrawLine(pos + Vector2.down * 0.5f, pos + (Right ? Vector2.left : Vector2.right) * 0.5f);
    }
}