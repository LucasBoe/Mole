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

    [SerializeField, ReadOnly] private bool isFallmodeActive = false;
    public bool IsFallmodeActive => isFallmodeActive;
    EnemyGroundCheckModule groundCheckModule;

    public bool IsStanding => rigidbody2D.rotation == 0 && !TriesStandingUp;

    public bool TriesStandingUp;

    private void Start()
    {
        groundCheckModule = GetModule<EnemyGroundCheckModule>();
        groundCheckModule.LeftGround += OnLeftGround;
    }

    public void Kick(Vector2 vector2)
    {
        SetFallmodeActive(true);
        groundCheckModule.ForceGroundedValue(false);
        rigidbody2D.AddForce(vector2, ForceMode2D.Impulse);
        Log($"receive kick rot( { rigidbody2D.rotation})");
    }

    private void OnLeftGround()
    {
        StopAllCoroutines();
        EndStandingUp();
    }

    internal void StartStandingUp()
    {
        Log("start standing up");
        TriesStandingUp = true;
        StopAllCoroutines();
        StartCoroutine(StandingUpRoutine());
    }

    private void EndStandingUp()
    {
        Log("finish standing up");
        rigidbody2D.simulated = true;
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
        isFallmodeActive = active;
        rigidbody2D.constraints = active ? RigidbodyConstraints2D.None : RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.drag = active ? 2 : 10;
        rigidbody2D.gravityScale = active ? 2 : 10;
        animator.enabled = !active;
        spriteRenderer.material = active ? fallMat : defaultMat;
        if (active)
        {
            spriteRenderer.sprite = fall_spritesheet;
        }
    }

    public void SetCollisionActive(bool active)
    {
        Log($"SetCollisionActive = { active }");
        bodyCollider.enabled = active;
    }

    private void FixedUpdate()
    {
        if (isFallmodeActive && rigidbody2D.velocity.magnitude > 1f)
        {
            float current = rigidbody2D.rotation;
            float target = Vector2.Angle(rigidbody2D.velocity, Vector2.left);
            float rotation = Mathf.MoveTowardsAngle(current, target, Time.fixedDeltaTime * (360f / 0.5f));

            var pos = transform.position;
            UnityEngine.Debug.DrawRay(new Vector2(pos.x + 0.1f, pos.y + 0.1f), Util.Vector2FromAngle(current), Color.red, Time.fixedDeltaTime);
            UnityEngine.Debug.DrawRay(new Vector2(pos.x + 0.0f, pos.y + 0.0f), Util.Vector2FromAngle(target), Color.green, Time.fixedDeltaTime);
            UnityEngine.Debug.DrawRay(new Vector2(pos.x - 0.1f, pos.y - 0.1f), Util.Vector2FromAngle(rotation), Color.yellow, Time.fixedDeltaTime);
            rigidbody2D.SetRotation(rotation);
        }
    }
}
