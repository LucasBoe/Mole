using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class EnemyPlayerDetectionModule : EnemyModule<EnemyPlayerDetectionModule>
{
    [SerializeField] private EnemyPlayerTrigger trigger;

    [SerializeField, ReadOnly] private bool IsPlayerInRange;
    [SerializeField] private float viewRange = 5f;
    [ReadOnly] public bool IsChecking;
    private EnemyNewMemoryModule memoryModule;

    private Coroutine checkBackOnPlayerRoutine, searchForPlayerRoutine;

    internal void StartChecking()
    {
        this.StopRunningCoroutine(searchForPlayerRoutine);
        searchForPlayerRoutine = StartCoroutine(SearchForPlayerRoutine());
        IsChecking = true;
    }

    internal void StopChecking()
    {
        this.StopRunningCoroutine(searchForPlayerRoutine);
        IsChecking = false;
    }

    private void Start()
    {
        memoryModule = GetModule<EnemyNewMemoryModule>();

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

    private void OnPlayerEnter(Collider2D playerCollider)
    {

        Rigidbody2D player = playerCollider.attachedRigidbody;

        if (CouldSee(player))
            FoundPlayer(player);

        checkBackOnPlayerRoutine = StartCoroutine(CheckBackOnPlayer(player));
        IsPlayerInRange = true;

    }

    private void OnPlayerExit(Collider2D playerCollider)
    {
        if (memoryModule.CanSeePlayer)
            LoosePlayer(playerCollider.attachedRigidbody);

        this.StopRunningCoroutine(checkBackOnPlayerRoutine);
        IsPlayerInRange = false;
    }


    private void FoundPlayer(Rigidbody2D player)
    {
        memoryModule.CanSeePlayer = true;
        memoryModule.IsAlerted = true;
        memoryModule.Player = player;
    }

    private void LoosePlayer(Rigidbody2D playerBody)
    {
        memoryModule.PlayerPos = playerBody.position;
        memoryModule.IsAlerted = true;
        memoryModule.CanSeePlayer = false;
    }

    private bool CouldSee(Rigidbody2D player)
    {
        float playerHiddenValue = PlayerHidingHandler.Instance.PlayerHiddenValue;
        bool VisibleInPlainSight = playerHiddenValue > 0.6f && Util.CheckLineOfSight(transform.position, player.position, "Default");
        bool VisibleInTwighlight = playerHiddenValue > 0.1f && Util.CheckLineOfSight(transform.position, player.position, new string[] { "Hangable", "Default" });
        bool playerIsInFrontOfEnemy = player.position.x < transform.position.x == (memoryModule.Forward == Direction2D.Left);

        return ((VisibleInPlainSight || VisibleInTwighlight) && playerIsInFrontOfEnemy);
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
            float length = viewRange;


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
            bool now = CouldSee(player);

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
        Gizmos.DrawWireSphere(transform.position, viewRange);
    }
}
