using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class EnemyPlayerDetectionModule : EnemyModule<EnemyPlayerDetectionModule>
{
    [SerializeField] private EnemyPlayerTrigger trigger;

    [SerializeField] private float viewRange = 5f;
    [ReadOnly] public bool IsChecking;
    private EnemyNewMemoryModule memoryModule;

    internal void StartChecking()
    {
        StopAllCoroutines();
        StartCoroutine(CheckSurroundingRoutine());
        IsChecking = true;
    }

    internal void StopChecking()
    {
        StopAllCoroutines();
        IsChecking = false;
    }

    private void Start()
    {
        memoryModule = GetModule<EnemyNewMemoryModule>();
        memoryModule.CheckedForPlayerPos += UpdatePlayerPos;

        trigger = Instantiate(trigger, transform);
        trigger.Init(viewRange);
        trigger.PlayerEnter += OnPlayerEnter;
        trigger.PlayerExit += OnPlayerExit;
    }

    private void OnDestroy()
    {
        trigger.PlayerEnter -= OnPlayerEnter;
        trigger.PlayerExit -= OnPlayerExit;
    }
    private void OnPlayerExit(Collider2D playerCollider)
    {
        memoryModule.PlayerPos = playerCollider.transform.position;
        memoryModule.CanSeePlayer = false;
        memoryModule.IsAlerted = true;
    }

    private void OnPlayerEnter(Collider2D playerCollider)
    {
        memoryModule.Player = playerCollider.attachedRigidbody;
        float playerHiddenValue = PlayerHidingHandler.Instance.PlayerHiddenValue;
        if (playerHiddenValue > 0.1f && Util.CheckLineOfSight(transform.position, playerCollider.attachedRigidbody.position, "Default"))
        {
            memoryModule.CanSeePlayer = true;
            memoryModule.IsAlerted = true;
        }
    }

    IEnumerator CheckSurroundingRoutine()
    {
        float offset = memoryModule.Forward == Direction2D.Right ? 0 : 180;

        yield return CheckRangeRoutine(offset + 0f, offset + 45f);
        yield return CheckRangeRoutine(offset + 45f, offset - 45f);

        yield return CheckRangeRoutine(offset + 180f, offset + 135f);
        yield return CheckRangeRoutine(offset + 135f, offset - 135f);

        memoryModule.IsAlerted = false;
        IsChecking = false;
    }

    private IEnumerator CheckRangeRoutine(float from, float to)
    {
        memoryModule.Forward = from < 90 || from > 270 ? Direction2D.Right : Direction2D.Left;
        while (Mathf.Abs(Mathf.DeltaAngle(from, to)) > 0.1f)
        {
            Vector2 start = transform.position;
            Vector2 direction = new Vector2(Mathf.Cos(from / Mathf.Rad2Deg), Mathf.Sin(from / Mathf.Rad2Deg));
            float length = viewRange;


            RaycastHit2D collider = Physics2D.Raycast(start, direction, length, LayerMask.GetMask("Player"));
            if (collider.collider != null && collider.collider.IsPlayer())
            {
                memoryModule.Player = collider.rigidbody;
                memoryModule.CanSeePlayer = true;
                Debug.DrawRay(start, direction * length, Color.green, 5);
            }
            else
                Debug.DrawRay(start, direction * length, Color.red, Time.deltaTime);

            from = Mathf.MoveTowardsAngle(from, to, Time.deltaTime * 45f);
            yield return null;
        }
    }

    private void UpdatePlayerPos(EnemyNewMemoryModule memory)
    {
        //
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRange);
    }
}
