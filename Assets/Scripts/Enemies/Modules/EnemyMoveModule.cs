using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAISmartType
{
    Lazy,
    Thorough,
}

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMoveModule : EnemyModule<EnemyMoveModule>
{
    CollisionCheck jumpHelperLeft, jumpHelperRight, ground, fallDetectionLeft, fallDetectionRight;

    List<CollisionCheck> collisionChecks = new List<CollisionCheck>();

    private Transform followTarget;
    private Vector2 moveTarget;
    private new Rigidbody2D rigidbody2D;

    public bool isMoving;
    public Vector2 MoveDir => rigidbody2D.velocity.normalized;

    public System.Action OnStartMovingToPosition;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        int mask = LayerMask.GetMask("Default", "Hangable");

        jumpHelperLeft = new CollisionCheck(-0.5f, -0.73f, 0.5f, 0.4f, mask, Color.yellow);
        collisionChecks.Add(jumpHelperLeft);

        jumpHelperRight = new CollisionCheck(0.5f, -0.73f, 0.5f, 0.4f, mask, Color.yellow);
        collisionChecks.Add(jumpHelperRight);

        ground = new CollisionCheck(0f, -1f, 0.5f, 0.2f, mask, Color.yellow);
        collisionChecks.Add(ground);

        fallDetectionLeft = new CollisionCheck(-1, -1f, 0.5f, 2.5f, mask, Color.red);
        collisionChecks.Add(fallDetectionLeft);

        fallDetectionRight = new CollisionCheck(1, -1f, 0.5f, 2.5f, mask, Color.red);
        collisionChecks.Add(fallDetectionRight);

    }

    internal void FollowTransform(Transform targetTransform)
    {
        followTarget = targetTransform;
        isMoving = true;
    }

    public void MoveTo(Vector2 position)
    {
        Util.DebugDrawCross(position, color: Color.red, 1, 3);
        moveTarget = position;
        isMoving = true;
        OnStartMovingToPosition?.Invoke();
    }

    public void StopMovingTo(Vector2 target)
    {
        if (moveTarget == target)
        {
            followTarget = null;
            moveTarget = Vector2.zero;
            isMoving = false;
        }
    }

    private void TargetReached()
    {
        StopMovingTo(moveTarget);
    }


    private void Update()
    {
        if (moveTarget == Vector2.zero)
            return;

        foreach (CollisionCheck cc in collisionChecks)
            cc.Update(transform);

        if (followTarget != null)
            moveTarget = followTarget.position;

        Vector2 dir = (moveTarget - (Vector2)transform.position).normalized;
        bool movingRight = dir.x > 0;

        rigidbody2D.AddForce((movingRight ? Vector2.right : Vector2.left) * 3000f * Time.deltaTime);

        if ((movingRight && jumpHelperRight.IsDetecting) || (!movingRight && jumpHelperLeft.IsDetecting))
            rigidbody2D.AddForce(Vector2.up, ForceMode2D.Impulse);

        if ((movingRight && !fallDetectionRight.IsDetecting) || (!movingRight && !fallDetectionLeft.IsDetecting))
            TargetReached();

        if (Mathf.Abs(transform.position.x - moveTarget.x) < 0.1f)
            TargetReached();
        
    }

    private void OnDrawGizmos()
    {
        foreach (CollisionCheck cc in collisionChecks)
            Util.GizmoDrawCollisionCheck(cc, transform.position);

        if (moveTarget != Vector2.zero)
            Gizmos.DrawWireSphere(moveTarget, 1f);

        if (rigidbody2D)
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)rigidbody2D.velocity.normalized);
    }
}
