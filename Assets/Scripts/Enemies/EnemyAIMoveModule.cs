using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAISmartType
{
    Lazy,
    Thorough,
}

public class EnemyAIMoveModule : MonoBehaviour
{
    CollisionCheck jumpHelperLeft, jumpHelperRight, ground;

    List<CollisionCheck> collisionChecks = new List<CollisionCheck>();

    Vector2 moveTarget;
    System.Action targetReachedCallback;
    Rigidbody2D rigidbody2D;

    public bool isMoving;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        jumpHelperLeft = new CollisionCheck(-0.3f, -0.73f, 0.3f, 0.3f, LayerMask.GetMask("Default", "Hangable"), Color.yellow);
        collisionChecks.Add(jumpHelperLeft);

        jumpHelperRight = new CollisionCheck(0.3f, -0.73f, 0.3f, 0.3f, LayerMask.GetMask("Default", "Hangable"), Color.yellow);
        collisionChecks.Add(jumpHelperRight);

        ground = new CollisionCheck(0f, -1f, 0.5f, 0.2f, LayerMask.GetMask("Default", "Hangable"), Color.yellow);
        collisionChecks.Add(ground);
    }

    public void MoveTo(Vector2 position, System.Action callback)
    {
        moveTarget = position;
        targetReachedCallback = callback;
        isMoving = true;
    }
    private void StopMoving()
    {
        targetReachedCallback?.Invoke();
        moveTarget = Vector2.zero;
        targetReachedCallback = null;
        isMoving = false;
    }


    private void Update()
    {
        if (moveTarget == Vector2.zero)
            return;

        foreach (CollisionCheck cc in collisionChecks)
            cc.Update(transform);


        Vector2 dir = (moveTarget - (Vector2)transform.position).normalized;
        rigidbody2D.AddForce((dir.x > 0 ? Vector2.right : Vector2.left) * 4000f * Time.deltaTime);

        if ((dir.x > 0 && jumpHelperRight.IsDetecting) || (dir.x < 0 && jumpHelperLeft.IsDetecting))
            rigidbody2D.AddForce(Vector2.up, ForceMode2D.Impulse);

        if (Mathf.Abs(transform.position.x - moveTarget.x) < 0.1f)
            StopMoving();
    }

    private void OnDrawGizmos()
    {
        foreach (CollisionCheck cc in collisionChecks)
            Util.GizmoDrawCollisionCheck(cc, transform.position);

        if (moveTarget != Vector2.zero)
            Gizmos.DrawWireSphere(moveTarget, 1f);
    }
}
