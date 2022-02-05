using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class Platform : MonoBehaviour
{
    [SerializeField, Layer] int blockviewLayer;
    bool seeThrough = false;
    EdgeCollider2D blockViewCollider;

    private void Awake()
    {
        GameObject viewBlocker = new GameObject("view-blocker-temp");
        viewBlocker.transform.parent = transform;
        viewBlocker.transform.localPosition = Vector3.zero;
        viewBlocker.layer = blockviewLayer;
        EdgeCollider2D collision = GetComponent<EdgeCollider2D>();
        blockViewCollider = viewBlocker.AddComponent<EdgeCollider2D>();
        blockViewCollider.offset = collision.offset;
        blockViewCollider.points = collision.points;
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool sneak = !PlayerInputHandler.PlayerInput.Sprinting;

        if (sneak != seeThrough)
            SetSeeThrough(sneak);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer() && collision.transform.position.y < transform.position.y)
            PlayerStateMachine.Instance.SetState(new PullUpState());
    }

    private void SetSeeThrough(bool value)
    {
        seeThrough = value;
        blockViewCollider.enabled = !seeThrough;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SetSeeThrough(false);
    }
}
