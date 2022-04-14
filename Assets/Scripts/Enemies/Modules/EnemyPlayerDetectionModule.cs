using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class EnemyPlayerDetectionModule : EnemyModule<EnemyPlayerDetectionModule>
{
    [SerializeField] private EnemyPlayerTrigger trigger;

    [SerializeField, ReadOnly] private bool IsPlayerInRange;
    [SerializeField] private float alertRange = 5f;
    [SerializeField] private float viewRange = 7f;
    [ReadOnly] public bool IsChecking;
    private EnemyMemoryModule memoryModule;

    private Coroutine checkBackOnPlayerRoutine, searchForPlayerRoutine;

    private void Start()
    {
        memoryModule = GetModule<EnemyMemoryModule>();
        memoryModule.AlertedEnter += OnAlert;

        trigger = Instantiate(trigger, transform);
        trigger.Init(alertRange);
        trigger.PlayerEnter += OnPlayerEnter;
    }

    internal void StartChecking()
    {
        Log("Start checking...");
        this.StopRunningCoroutine(searchForPlayerRoutine);
        searchForPlayerRoutine = StartCoroutine(SearchForPlayerRoutine());
        IsChecking = true;
    }
    private void OnAlert()
    {
        if (IsChecking)
            StopChecking();
    }

    internal void StopChecking()
    {
        this.StopRunningCoroutine(searchForPlayerRoutine);
        IsChecking = false;
    }

    private void OnDestroy()
    {
        trigger.PlayerEnter -= OnPlayerEnter;
    }

    private void OnPlayerEnter(Rigidbody2D player)
    {
        if (CouldSee(player))
            FoundPlayer(player);

        this.StopRunningCoroutine(checkBackOnPlayerRoutine);
        checkBackOnPlayerRoutine = StartCoroutine(CheckBackOnPlayer(player));
        IsPlayerInRange = true;

    }

    private void SetPlayerOutOfRange(Rigidbody2D player)
    {
        if (memoryModule.CanSeePlayer)
            LoosePlayer(player);

        this.StopRunningCoroutine(checkBackOnPlayerRoutine);
        IsPlayerInRange = false;
    }


    private void FoundPlayer(Rigidbody2D player)
    {
        memoryModule.CanSeePlayer = true;
        memoryModule.Alert();
        memoryModule.Player = player;
    }

    private void LoosePlayer(Rigidbody2D playerBody)
    {
        memoryModule.Alert(playerBody.position);
        memoryModule.CanSeePlayer = false;
    }

    private bool CouldSee(Rigidbody2D player, float drawLineOfSightWith = -1)
    {
        bool playerIsInRange = Vector2.Distance(transform.position, player.position) <= viewRange;

        if (!playerIsInRange)
        {
            SetPlayerOutOfRange(player);
            return false;
        }

        float playerHiddenValue = PlayerHidingHandler.Instance.PlayerHiddenValue;
        bool playerIsInFrontOfEnemy = Direction2DUtil.FromPositions(transform.position, player.position) == memoryModule.Forward;
        bool playerIsInLineOfSight = Util.CheckLineOfSight(transform.position, player.position, new string[] { "Hangable", "HangableCollidable", "Default" }, drawLineOfSightWith);
        bool visibleInPlainSight = playerHiddenValue > 0.6f && playerIsInLineOfSight;
        bool visibleInTwighlight = playerHiddenValue > 0.1f && playerIsInLineOfSight;

        return (playerIsInRange && playerIsInFrontOfEnemy && (visibleInPlainSight || visibleInTwighlight));
    }

    IEnumerator SearchForPlayerRoutine()
    {
        float offset = memoryModule.Forward == Direction2D.Right ? 0 : 180;

        yield return SearchInAgleRoutine(offset + 0f, offset + 45f);
        yield return SearchInAgleRoutine(offset + 45f, offset - 45f);

        yield return SearchInAgleRoutine(offset + 180f, offset + 135f);
        yield return SearchInAgleRoutine(offset + 135f, offset - 135f);

        memoryModule.IsAlerted = false;
        IsChecking = false;
    }

    private IEnumerator SearchInAgleRoutine(float from, float to)
    {
        memoryModule.Forward = from < 90 || from > 270 ? Direction2D.Right : Direction2D.Left;
        while (Mathf.Abs(Mathf.DeltaAngle(from, to)) > 0.1f)
        {
            Vector2 start = transform.position;
            Vector2 direction = Util.Vector2FromAngle(from);
            float length = alertRange;


            RaycastHit2D collider = Physics2D.Raycast(start, direction, length, LayerMask.GetMask("Player"));
            if (collider.collider != null && collider.collider.IsPlayer() && CouldSee(collider.rigidbody))
            {
                FoundPlayer(collider.rigidbody);
                UnityEngine.Debug.DrawRay(start, direction * length, Color.green, 5);
            }
            else
                UnityEngine.Debug.DrawRay(start, direction * length, Color.red, Time.deltaTime);

            from = Mathf.MoveTowardsAngle(from, to, Time.deltaTime * 45f);
            yield return null;
        }
    }

    private IEnumerator CheckBackOnPlayer(Rigidbody2D player)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            bool before = memoryModule.CanSeePlayer;
            bool now = CouldSee(player, drawLineOfSightWith: 0.5f);

            if (before != now)
            {
                if (now == false)
                    LoosePlayer(player);
                else
                    FoundPlayer(player);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);
    }
}
