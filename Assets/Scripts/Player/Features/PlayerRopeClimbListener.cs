using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRopeClimbListener : SingletonBehaviour<PlayerRopeClimbListener>
{
    public enum States
    {
        Active,
        Passive,
    }

    States currentState = States.Passive;
    Coroutine currentCoroutine;

    private void Update()
    {
        if (currentState == States.Passive)
            return;

        Collider2D ropeCollider = Physics2D.OverlapBox(transform.position, Vector2.one, 0, LayerMask.GetMask("Rope"));

        if (ropeCollider != null)
        {
            RopeElement rope = ropeCollider.GetComponent<RopeElement>();

            if (rope != null)
            {
                PlayerStateMachine.Instance.SetState(new RopeClimbState(rope));
                return;
            }
        }
    }

    public void TrySetState(States toSet, float delay = -1, bool forceOverride = false)
    {
        if (currentCoroutine != null && !forceOverride)
            return;

        if (forceOverride)
            StopAllCoroutines();

        if (delay <= 0)
        {
            currentState = toSet;
        } else
        {
            currentCoroutine = StartCoroutine(SetStateDelayedRoutine(toSet, delay));
        }
    }

    private IEnumerator SetStateDelayedRoutine(States toSet, float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = toSet;
        currentCoroutine = null;
    }
}
