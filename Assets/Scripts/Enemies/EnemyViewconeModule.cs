using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewconeModule : EnemyModule<EnemyViewconeModule>
{
    [SerializeField] LineRenderer viewConeLines;
    [SerializeField] PolygonCollider2D polygonCollider2D;

    public System.Action<Transform> OnPlayerEnter;
    public System.Action<Vector2> OnPlayerExit;


    internal void UpdateBounds(Vector2 eyePosition, float viewConeDistance, float viewConeHeight)
    {
        transform.localPosition = eyePosition;

        Vector2[] points = new Vector2[] { Vector2.right * viewConeDistance + Vector2.up * (viewConeHeight / 2f),
            Vector2.zero,
            Vector2.right * viewConeDistance + Vector2.down * (viewConeHeight / 2f) };

        polygonCollider2D.points = points;
        viewConeLines.positionCount = points.Length;
        viewConeLines.SetPositions(points.ToVector3Array());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            OnPlayerEnter?.Invoke(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            OnPlayerExit?.Invoke(collision.transform.position);
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
