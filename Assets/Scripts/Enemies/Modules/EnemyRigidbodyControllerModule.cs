using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRigidbodyControllerModule : EnemyModule<EnemyRigidbodyControllerModule>, ICollisionModifier
{
    [SerializeField] Collider2D bodyCollider;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite fall_spritesheet;
    [SerializeField] Material defaultMat, fallMat;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] Animator animator;

    public System.Action<bool> FallmodeChanged;

    private bool falling = false;

    public void SetFallmodeActive(bool active)
    {
        rigidbody2D.constraints = active ? RigidbodyConstraints2D.None : RigidbodyConstraints2D.FreezeRotation;
        animator.enabled = !active;
        spriteRenderer.material = active ? fallMat : defaultMat;
        if (active)
        {
            spriteRenderer.sprite = fall_spritesheet;
            transform.rotation = Quaternion.identity;
        }
        falling = active;
        FallmodeChanged?.Invoke(active);
    }

    public void SetCollisionActive(bool active)
    {
        bodyCollider.enabled = active;
    }

    private void FixedUpdate()
    {
        if (falling)
        {
            float current = rigidbody2D.rotation;
            float target = Vector2.Angle(rigidbody2D.velocity, Vector2.left);
            Mathf.MoveTowardsAngle(current, target, Time.deltaTime);
            rigidbody2D.SetRotation(target);
        }

        Time.timeScale = falling ? 0.25f : 1f;
    }
}
