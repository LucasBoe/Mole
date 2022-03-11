using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheckModule : EnemyModule<EnemyGroundCheckModule>
{

    [SerializeField, ReadOnly] private bool isGrounded = true;
    [SerializeField] bool debug;
    public bool IsGrounded => isGrounded;

    public System.Action EnteredGround;
    public System.Action LeftGround;
    private void Start()
    {
        StartCoroutine(RaycastForGroundRoutine());
    }

    IEnumerator RaycastForGroundRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            var pos1 = new Vector2(transform.position.x - 0.5f, transform.position.y);
            var pos2 = new Vector2(transform.position.x + 0.5f, transform.position.y);
            var length = 2;

            bool before = isGrounded;
            isGrounded = DoRaycast(pos1, length) || DoRaycast(pos2, length);

            UnityEngine.Debug.DrawRay(pos1, Vector2.down * length, isGrounded ? Color.green : Color.red, 0.25f);
            UnityEngine.Debug.DrawRay(pos2, Vector2.down * length, isGrounded ? Color.green : Color.red, 0.25f);

            if (before != isGrounded)
            {
                if (!isGrounded) LeftGround?.Invoke();
                else EnteredGround?.Invoke();
            }
        }
    }

    internal void ForceGroundedValue(bool newValue)
    {
        isGrounded = false;
    }

    private bool DoRaycast(Vector3 pos, int length)
    {

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, length, LayerMask.GetMask("Default", "Hangable"));
        return hit.collider != null;
    }
}
