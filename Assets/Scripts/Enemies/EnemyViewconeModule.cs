using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewconeModule : EnemyModule<EnemyViewconeModule>
{
    public enum ViewconeMode
    {
        Free,
        FollowTransform,
        ScanSurrounding,
        LookForward,
    }

    private enum TriggerMode
    {
        InnerCollider,
        OuterCollider,
    }

    [SerializeField] LineRenderer viewConeLines;
    [SerializeField] PolygonCollider2D inner, outer;

    public System.Action<Transform> OnPlayerEnter;
    public System.Action<Vector2> OnPlayerExit;

    [SerializeField] ViewconeMode viewconeMode;
    TriggerMode mode = TriggerMode.InnerCollider;

    Coroutine doneLookingCoroutine;
    public bool Done => doneLookingCoroutine == null;
    public System.Action OnStartLookingAround;

    private bool canSeeTarget;
    public bool CanSeeTarget { get => canSeeTarget; }
    private Vector3 lastSeenTarget;
    public Vector3 LastSeenTarget { get => lastSeenTarget; }
    [SerializeField] private int lookedAroundCounter;
    public int LookedAroundCounter { get => lookedAroundCounter; }
    public bool IsLookingLeft { get => isAngleLeft(transform.rotation.eulerAngles.z); }
    public bool IsPassive = false;

    EnemyMoveModule moveModule;
    [SerializeField] Vector3 lastPos;
    [SerializeField] float currentAngle;
    [SerializeField] float targetAngle;
    Transform targetTransform;

    private void Start()
    {
        moveModule = GetModule<EnemyMoveModule>();
        viewconeMode = ViewconeMode.LookForward;
        SetViewconeTriggerMode(TriggerMode.InnerCollider);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer() && CheckLineOfSight(collision.transform) && !IsPassive)
        {
            SetTarget(collision.transform);
            OnPlayerEnter?.Invoke(collision.transform);
            SetViewconeTriggerMode(TriggerMode.OuterCollider);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (mode == TriggerMode.OuterCollider && collision.IsPlayer())
        {
            OnPlayerExit?.Invoke(collision.transform.position);
        }
    }

    private void Update()
    {
        switch (viewconeMode)
        {
            case ViewconeMode.LookForward:
                if (moveModule != null)
                    targetAngle = GetAngleFromDir(-moveModule.MoveDir);
                break;

            case ViewconeMode.FollowTransform:
                Vector3 t = targetTransform && canSeeTarget ? targetTransform.position : lastSeenTarget;
                targetAngle = GetAngleFromTarget(t);
                break;
        }

        //bool canSeeTargetBefore = canSeeTarget;
        canSeeTarget = CheckLineOfSight();

        if (canSeeTarget && viewconeMode != ViewconeMode.FollowTransform)
            SetViewconeMode(ViewconeMode.FollowTransform);

        if (isAngleLeft(currentAngle) && !isAngleLeft(targetAngle))
        {
            float angleDifference = (Mathf.DeltaAngle(currentAngle, 90) * 2f);
            currentAngle = Util.FixAngle(currentAngle - angleDifference);
        }
        else if (!isAngleLeft(currentAngle) && isAngleLeft(targetAngle))
        {
            float angleDifference = (Mathf.DeltaAngle(currentAngle, 90) * 2f);
            currentAngle = Util.FixAngle(currentAngle + angleDifference);
        }

        currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, Time.deltaTime * 45);
        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

        if (targetTransform != null)
            lastSeenTarget = targetTransform.position;

        if (Vector2.Distance(lastPos, transform.position) > 0.1f)
            lastPos = transform.position;

        //Vector2 pos = PlayerInputHandler.PlayerInput.VirtualCursorToWorldPos;
        //targetAngle = GetAngleFromTarget(pos);
        //Debug.DrawLine(transform.position, pos);
    }

    private IEnumerator LookAroundRoutine()
    {
        OnStartLookingAround?.Invoke();

        targetAngle = 45;
        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f)
        {
            yield return null;
        }

        targetAngle = -25;
        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f)
        {
            yield return null;
        }

        targetAngle = 0;
        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f)
        {
            yield return null;
        }

        lookedAroundCounter++;
        doneLookingCoroutine = null;
    }

    internal void SetTarget(Transform target)
    {
        targetTransform = target;
        SetViewconeMode(ViewconeMode.FollowTransform);
    }

    private void SetViewconeTriggerMode(TriggerMode mode)
    {
        inner.enabled = mode == TriggerMode.InnerCollider;
        outer.enabled = mode == TriggerMode.OuterCollider;
        this.mode = mode;
    }

    public void SetViewconeMode(ViewconeMode viewconeMode)
    {
        if (viewconeMode != ViewconeMode.FollowTransform)
            targetTransform = null;

        this.viewconeMode = viewconeMode;
        lookedAroundCounter = 0;

        if (doneLookingCoroutine != null)
            StopCoroutine(doneLookingCoroutine);

        if (viewconeMode == ViewconeMode.ScanSurrounding)
        {
            doneLookingCoroutine = StartCoroutine(LookAroundRoutine());
        }
    }

    private float GetAngleFromDir(Vector2 dir)
    {
        return Vector2.Angle(Vector2.left * transform.parent.localScale.x, dir);
    }

    private float GetAngleFromTarget(Vector3 target)
    {
        int correction = (target.y < transform.position.y) ? -1 : 1;
        return GetAngleFromDir(GetDirFromTarget(target)) * correction;
    }

    private Vector2 GetDirFromTarget(Vector3 target)
    {
        return (transform.position - target).normalized;
    }

    public void Look(LookDirection direction)
    {
        targetAngle = direction == LookDirection.Left ? 180 : 0;
        viewconeMode = ViewconeMode.Free;
    }

    public void ResetCollider()
    {
        SetViewconeTriggerMode(TriggerMode.InnerCollider);
    }
    private bool CheckLineOfSight(Transform customTarget = null)
    {
        if (customTarget == null && targetTransform == null)
            return false;

        Transform target = customTarget != null ? customTarget : targetTransform;

        float maxDist = 10;

        if (Vector2.Distance(transform.position, target.position) > maxDist)
            return false;

        return Util.CheckLineOfSight(transform.position, target.position, "Default");
    }

    private bool isAngleLeft(float angle)
    {
        return (angle > 90 && angle < 270) || (angle < -90 && angle > 270);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.left * 2 * (IsLookingLeft ? 1 : -1), Vector3.one * 2);
    }
}
