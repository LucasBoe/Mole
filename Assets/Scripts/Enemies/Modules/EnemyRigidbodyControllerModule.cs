using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRigidbodyControllerModule : EnemyModule<EnemyRigidbodyControllerModule>, ICollisionModifier
{
    [SerializeField] CapsuleCollider2D bodyCollider;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite fall_spritesheet;
    [SerializeField] Material defaultMat, fallMat;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] Animator animator;

    public System.Action<bool> FallmodeChanged;

    [SerializeField, ReadOnly] private bool isFalling = false;
    public bool IsFalling => isFalling;

    public bool IsStanding = false;
    public bool TriesStandingUp;

    private void Start()
    {
        GetModule<EnemyGroundCheckModule>().LeftGround += OnLeftGround;
    }

    private void OnLeftGround()
    {
        StopAllCoroutines();
        EndStandingUp();
        IsStanding = false;
    }

    internal void StartStandingUp()
    {
        TriesStandingUp = true;
        StopAllCoroutines();
        StartCoroutine(StandingUpRoutine());
    }

    private void EndStandingUp()
    {
        rigidbody2D.simulated = true;
        IsStanding = true;
        TriesStandingUp = false;
    }

    private IEnumerator StandingUpRoutine()
    {
        rigidbody2D.simulated = false;
        Vector2 targetPos = new Vector2(rigidbody2D.position.x, rigidbody2D.position.y);
        while (Physics2D.OverlapCapsule(targetPos, bodyCollider.size, CapsuleDirection2D.Vertical, 0, LayerMask.GetMask("Default", "Hangable")))
            targetPos += Vector2.up * 0.25f;
        

        transform.position = targetPos;
        transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(1f);
        EndStandingUp();
    }

    public void SetFallmodeActive(bool active)
    {
        isFalling = active;
        rigidbody2D.constraints = active ? RigidbodyConstraints2D.None : RigidbodyConstraints2D.FreezeRotation;
        animator.enabled = !active;
        spriteRenderer.material = active ? fallMat : defaultMat;
        if (active)
        {
            spriteRenderer.sprite = fall_spritesheet;
        }
        FallmodeChanged?.Invoke(active);
    }

    public void SetCollisionActive(bool active)
    {
        bodyCollider.enabled = active;
    }

    private void FixedUpdate()
    {
        if (isFalling && rigidbody2D.velocity.magnitude > 1f)
        {
            float current = rigidbody2D.rotation;
            float target = Vector2.Angle(rigidbody2D.velocity, Vector2.left);
            float rotation = Mathf.MoveTowardsAngle(current, target, Time.fixedDeltaTime * (360f / 0.5f));

            var pos = transform.position;
            Debug.DrawRay(new Vector2(pos.x + 0.1f, pos.y + 0.1f), Util.Vector2FromAngle(current), Color.red, Time.fixedDeltaTime);
            Debug.DrawRay(new Vector2(pos.x + 0.0f, pos.y + 0.0f), Util.Vector2FromAngle(target), Color.green, Time.fixedDeltaTime);
            Debug.DrawRay(new Vector2(pos.x - 0.1f, pos.y - 0.1f), Util.Vector2FromAngle(rotation), Color.yellow, Time.fixedDeltaTime);
            rigidbody2D.SetRotation(rotation);
        }
    }
}
