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

    [SerializeField, ReadOnly] EnemyBase enemy;
    [SerializeField, ReadOnly, Range(0, 10)] float speed = 0f;
    [SerializeField, ReadOnly] float idleTime;
    [SerializeField, ReadOnly] bool unconscious = false;

    public void Connect(EnemyBase enemy)
    {
        this.enemy = enemy;
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
                unconscious = true;

        }

        if (idleTime > 3)
            StandUp();
    }

    private void StandUp()
    {
        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;
        enemy.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
