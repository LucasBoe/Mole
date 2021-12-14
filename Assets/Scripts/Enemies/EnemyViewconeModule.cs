using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewconeModule : EnemyModule<EnemyViewconeModule>
{
    private enum TriggerMode
    {
        InnerCollider,
        OuterCollider,
    }

    [SerializeField] LineRenderer viewConeLines;
    [SerializeField] PolygonCollider2D inner, outer;

    public System.Action<Transform> OnPlayerEnter;
    public System.Action<Vector2> OnPlayerExit;

    TriggerMode mode = TriggerMode.InnerCollider;

    private void Start()
    {
        SetViewconeTriggerMode(TriggerMode.InnerCollider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            OnPlayerEnter?.Invoke(collision.transform);
            SetViewconeTriggerMode(TriggerMode.OuterCollider);
        }
    }

    private void SetViewconeTriggerMode(TriggerMode mode)
    {
        inner.enabled = mode == TriggerMode.InnerCollider;
        outer.enabled = mode == TriggerMode.OuterCollider;
        this.mode = mode;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (mode == TriggerMode.OuterCollider && collision.IsPlayer())
        {
            OnPlayerExit?.Invoke(collision.transform.position);
        }
    }

    public void ResetCollider()
    {
        SetViewconeTriggerMode(TriggerMode.InnerCollider);
    }

    internal void LookTo(Vector2 target, bool andBack = false, Action callback = null)
    {
        StartCoroutine(LookToRoutine(target, andBack, callback));
    }

    public void ResetRotation ()
    {
        transform.localRotation = Quaternion.identity;
    }

    private IEnumerator LookToRoutine(Vector2 target, bool andBack, Action callback)
    {
        float baseRot = 0;
        float targetRot = Vector2.Angle(Vector2.right, (target - (Vector2)transform.position).normalized);
        float currentRot = 0;

        float lookAroundSpeed = 100f;

        while (Mathf.Abs(currentRot - targetRot) > 0.1f)
        {
            currentRot = Mathf.MoveTowardsAngle(currentRot, targetRot, Time.deltaTime * lookAroundSpeed);
            transform.localRotation = Quaternion.Euler(0,0, currentRot);
            yield return null;
        }

        if (andBack)
        {
            while (Mathf.Abs(currentRot - baseRot) > 0.1f)
            {
                currentRot = Mathf.MoveTowardsAngle(currentRot, baseRot, Time.deltaTime * lookAroundSpeed);
                transform.localRotation = Quaternion.Euler(0, 0, currentRot);
                yield return null;
            }
        }

        callback?.Invoke();
    }
}
