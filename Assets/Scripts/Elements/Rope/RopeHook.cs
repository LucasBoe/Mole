using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeHook : MonoBehaviour
{
    public enum States
    {
        Throw,
        MoveBack,
        Static,
    }

    [SerializeField] private Rigidbody2D rigidbody2D;

    private States current = States.Throw;
    private Vector2 startPos;
    private float lastDistance;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (current == States.Static)
            return;
        else if (current == States.Throw)
        {
            float distanceToStart = Vector2.Distance(startPos, transform.position);

            if (distanceToStart >= lastDistance)
            {
                lastDistance = distanceToStart;
            }
            else
            {
                current = States.MoveBack;
            }
        }
        else
        {
            if (Physics2D.Raycast(transform.position, Vector2.down, 1, LayerMask.GetMask("Default")))
            {
                FixateHook();
                current = States.Static;
            }
        }
    }

    private void FixateHook()
    {
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        RopeHandler.Instance.CreateRope(PlayerController.Context.Rigidbody, rigidbody2D);
    }
}
