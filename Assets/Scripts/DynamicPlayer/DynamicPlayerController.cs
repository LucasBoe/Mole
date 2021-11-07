using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DynamicPlayerController : MonoBehaviour
{
    [SerializeField] PlayerCollisionCheck footCheck, leftCheck, rightCheck, ceilingCheck;
    [SerializeField] bool grounded;
    [SerializeField] Limb leg1, leg2, arm1, arm2;
    [SerializeField] DynamicPlayerBody body;

    private void Start()
    {
        leg1.Parner = leg2;
        leg2.Parner = leg1;

        arm1.Parner = arm2;
        arm2.Parner = arm1;

        leg1.Current = leg2.Target;
        arm1.Current = arm2.Target;
    }

    private void Update()
    {
        footCheck.Update(transform);
        leftCheck.Update(transform);
        rightCheck.Update(transform);
        ceilingCheck.Update(transform);

        Rigidbody2D rigidbody2D;
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = leftCheck.IsDetecting || rightCheck.IsDetecting || ceilingCheck.IsDetecting ? 0 : 1;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (footCheck.IsDetecting || leftCheck.IsDetecting || rightCheck.IsDetecting || ceilingCheck.IsDetecting)
            rigidbody2D.velocity = Vector2.Lerp(rigidbody2D.velocity, input * 2, 0.5f);

        //rigidbody2D.AddForce(input * Time.deltaTime *100f ,ForceMode2D.Impulse);



        body.UpdateBody(footCheck, leftCheck, rightCheck, ceilingCheck, input.x);

        if (footCheck.IsDetecting)
        {
            leg1.Target = new Vector2(transform.position.x - 0.25f, transform.position.y - 0.5f);
            leg2.Target = new Vector2(transform.position.x + 0.25f, transform.position.y - 0.5f);

            if (leftCheck.IsDetecting || rightCheck.IsDetecting)
            {
                float x = leftCheck.IsDetecting ? Mathf.Floor(transform.position.x) : Mathf.Ceil(transform.position.x);
                ArmsToWall(x);
            }
            else
            {
                arm1.Free(transform);
                arm2.Free(transform);
            }
        }
        else if (leftCheck.IsDetecting || rightCheck.IsDetecting)
        {
            float x = leftCheck.IsDetecting ? Mathf.Floor(transform.position.x) : Mathf.Ceil(transform.position.x);
            LegsToWall(x);

            if (!ceilingCheck.IsDetecting)
            {
                ArmsToWall(x);
            }
            else
            {
                float dir = leftCheck.IsDetecting ? 1 : -1;
                ArmsToCeiling(dir);
            }
        }
        else
        {
            if (ceilingCheck.IsDetecting)
            {
                float dir = leftCheck.IsDetecting ? 1 : -1;
                LegsToCeiling(dir);
                ArmsToCeiling(dir);
            }
            else
            {
                arm1.Free(transform);
                arm2.Free(transform);
                leg1.Free(transform);
                leg2.Free(transform);
            }
        }

        arm1.Update();
        arm2.Update();
        leg1.Update();
        leg2.Update();
    }


    private void LegsToCeiling(float dir)
    {
        float y = Mathf.Ceil(transform.position.y);
        leg1.Target = new Vector2(transform.position.x - dir * 0.25f, y);
        leg2.Target = new Vector2(transform.position.x - dir * 0.75f, y);
    }

    private void LegsToWall(float x)
    {
        leg1.Target = new Vector2(x, transform.position.y - 0.25f);
        leg2.Target = new Vector2(x, transform.position.y - 0.75f);
    }

    private void ArmsToCeiling(float dir)
    {
        float y = Mathf.Ceil(transform.position.y);
        arm1.Target = new Vector2(transform.position.x + dir * 0.25f, y);
        arm2.Target = new Vector2(transform.position.x + dir * 0.75f, y);
    }

    private void ArmsToWall(float x)
    {
        arm1.Target = new Vector2(x, transform.position.y + 0.25f);
        arm2.Target = new Vector2(x, transform.position.y + 0.75f);
    }

    private void OnDrawGizmosSelected()
    {
        DrawCollisionCheck(footCheck, Color.blue);
        DrawCollisionCheck(leftCheck, Color.yellow);
        DrawCollisionCheck(rightCheck, Color.yellow);
        DrawCollisionCheck(ceilingCheck, Color.cyan);

        Gizmos.DrawWireSphere(leg1.Target, 0.1f);
        Gizmos.DrawWireSphere(leg2.Target, 0.1f);
        Gizmos.DrawWireSphere(arm1.Target, 0.1f);
        Gizmos.DrawWireSphere(arm2.Target, 0.1f);
    }

    private void DrawCollisionCheck(PlayerCollisionCheck toDraw, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawWireCube((Vector2)transform.position + toDraw.pos, toDraw.size);
    }
}

[System.Serializable]
public class PlayerCollisionCheck
{
    public LayerMask layerMask;
    public Vector2 pos, size = Vector2.one;
    public bool IsDetecting = false;

    public void Update(Transform player)
    {
        IsDetecting = Physics2D.OverlapBox((Vector2)player.position + pos, size, 0, layerMask) != null;
    }
}

[System.Serializable]
public class Limb
{
    public Limb Parner;
    public Transform Origin;
    public Vector2 Target;
    public Vector2 Current;

    public bool IsCorrecting = false;

    public void Update()
    {
        if (Vector2.Distance(Current, Target) > 0.5f && Parner.IsCorrecting == false)
            IsCorrecting = true;

        if (IsCorrecting)
        {
            Current = Vector2.MoveTowards(Current, Target, Time.deltaTime * 4f);
            if (Vector2.Distance(Current, Target) < 0.01f)
                IsCorrecting = false;
        }

        Origin.up = (Origin.position - (Vector3)Current).normalized;
    }

    internal void Free(Transform transform)
    {
        Target = transform.position;
    }
}
