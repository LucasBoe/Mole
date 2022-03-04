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

    [SerializeField, ReadOnly] private bool isFalling = false;
    public bool IsFalling => isFalling;

    public void SetFallmodeActive(bool active)
    {
        isFalling = active;
        rigidbody2D.constraints = active ? RigidbodyConstraints2D.None : RigidbodyConstraints2D.FreezeRotation;
        animator.enabled = !active;
        spriteRenderer.material = active ? fallMat : defaultMat;
        if (active)
        {
            spriteRenderer.sprite = fall_spritesheet;
        } else
        {
            transform.rotation = Quaternion.identity;
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
