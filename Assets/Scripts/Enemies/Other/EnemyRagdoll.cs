using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class EnemyRagdoll : MonoBehaviour
{
    [Foldout("References"), SerializeField] Rigidbody2D rigidbody;
    [Foldout("References"), SerializeField] SpriteRenderer spriteRenderer;
    [Foldout("References"), SerializeField] Sprite fall, idle;
    [Foldout("References"), SerializeField] ParticleSystem unconciousEffect;
    [Foldout("References"), SerializeField] Collider2D[] colliders;
    [Foldout("References"), SerializeField] CarriablePlayerItemWorldObject carry;

    [SerializeField, ReadOnly] EnemyBase enemy;
    [SerializeField, ReadOnly, Range(0, 10)] float speed = 0f;
    [SerializeField, ReadOnly] float idleTime;
    [SerializeField, ReadOnly] bool unconscious = false;
    [SerializeField, ReadOnly] EnemyLootModule lootModule;

    public static System.Action<EnemyRagdoll, bool> ChangedUnconcious;

    private void OnEnable()
    {
        carry.StartCarryThis += OnStartCarry;
    }

    private void OnStartCarry()
    {
        if (lootModule != null) lootModule.Loot.PlayerTryLoot();
    }

    internal void Hide(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPositonAndHide(position));
    }

    private IEnumerator MoveToPositonAndHide(Vector3 position)
    {
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        while (Vector2.Distance(transform.position, position) > 0.1f)
        {
            rigidbody.MovePosition(Vector2.MoveTowards(rigidbody.position, position, Time.deltaTime * 25f));
            yield return null;
        }

        Destroy(gameObject);
    }
    public void Connect(EnemyBase enemy)
    {
        this.enemy = enemy;
        lootModule = enemy.GetModule<EnemyLootModule>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = rigidbody.velocity.magnitude;
        spriteRenderer.sprite = speed > 3 ? fall : idle;

        bool isMoving = speed > 0.1f;
        idleTime = isMoving ? 0 : idleTime += Time.deltaTime;

        if (!unconscious)
        {
            if (speed > 10f)
                SetUnconscious();

            if (idleTime > 3)
                StandUp();
        }

    }

    private void SetUnconscious()
    {
        unconscious = true;
        ChangedUnconcious?.Invoke(this, true);
        StartCoroutine(UnconsciousRoutine());
    }

    private void OnDestroy()
    {
        ChangedUnconcious?.Invoke(this, false);
    }

    private void StandUp()
    {
        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;
        enemy.gameObject.SetActive(true);
        Destroy(gameObject);
    }

    private IEnumerator UnconsciousRoutine()
    {
        while (true)
        {
            EffectHandler.Spawn(new CustomEffect(unconciousEffect, 4), transform.position);
            yield return new WaitForSeconds(2);
        }
    }
}
