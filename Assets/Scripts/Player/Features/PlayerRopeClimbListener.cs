using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRopeClimbListener : SingletonBehaviour<PlayerRopeClimbListener>
{
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] SpringJoint2D springJoint2D;
    public enum States
    {
        Idle,
        Climb,
    }

    [SerializeField] States currentState = States.Idle;
    Coroutine currentCoroutine;
    bool hover;

    private void Update()
    {
        Collider2D[] ropeColliders = Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0, LayerMask.GetMask("Rope"));

        bool hoverBefore = hover;
        hover = ropeColliders.Length > 0;
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
            springJoint2D.enabled = toSet == States.Climb;
        }
        else
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

    internal void SetClimbingBody(Rigidbody2D body)
    {
        springJoint2D.connectedBody = body;
    }

    internal ColliderDistance2D GetDistanceToBody()
    {
        return springJoint2D.connectedBody.Distance(PlayerColliderModifier.Instance.GetActiveCollider());
    }
}
