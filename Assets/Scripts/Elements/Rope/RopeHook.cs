using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeHook : CollectablePlayerItemWorldObject
{
    public enum States
    {
        Throw,
        MoveBack,
        Static,
    }

    [SerializeField] new private Rigidbody2D rigidbody2D;
    [SerializeField] new private Collider2D collider;
    [SerializeField] private LineRenderer lineRenderer;

    private States current = States.Throw;
    private List<Vector2> travelPositions = new List<Vector2>();

    private void Start()
    {
        travelPositions.Add(transform.position);
    }

    private void Update()
    {
        if (current == States.Static)
            return;
        else if (current == States.Throw)
        {
            /*    float distanceToStart = Vector2.Distance(startPos, transform.position);

                if (distanceToStart >= lastDistance)
                {
                    lastDistance = distanceToStart;
                }
                else
                {
                    current = States.MoveBack;
                }
            }
            else
            {
            */

            if (Vector2.Distance(transform.position, travelPositions.Last()) >= 1)
            {
                Vector3 pos = transform.position + Vector3.up;
                travelPositions.Add(pos);
                Util.DebugDrawCircle(pos, Color.green, 0.5f, lifetime: 3f);
            }

            int mask = LayerMask.GetMask(CollisionCheckUtil.GetLineOfSightMask());

            if (Physics2D.Raycast(new Vector2(transform.position.x - 0.1f, transform.position.y + 0.2f), Vector2.down, 0.5f, mask)
                || Physics2D.Raycast(new Vector2(transform.position.x + 0.1f, transform.position.y + 0.2f), Vector2.down, 0.5f, mask))
            {
                FixateHook();
                current = States.Static;
            }

            UpateLineRenderer();
        }
    }

    private void UpateLineRenderer()
    {
        Vector3[] pos = InterpolatePathWihPlayer(travelPositions.ToArray()).ToVector3Array();

        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions(pos);
    }

    private Vector2[] InterpolatePathWihPlayer(Vector2[] path)
    {
        int length = path.Length;
        Vector2[] pos = new Vector2[length];
        string str = "";
        for (int i = 0; i < pos.Length; i++)
        {
            float lerpValue = Mathf.Min((float)i * 2 / Mathf.Max((length) - 1, 1), 1);
            pos[i] = Vector2.Lerp(PlayerController.Instance.transform.position, path[i], lerpValue);
        }

        Debug.Log(str);

        return pos;
    }

    private void FixateHook()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        collider.enabled = false;
        travelPositions.Add(transform.position);
        Destroy(lineRenderer);
        CableHandler.Instance.CreateRope(PlayerController.Context.Rigidbody, rigidbody2D, pathPoints: InterpolatePathWihPlayer(travelPositions.ToArray()));
    }

    public override bool Collect()
    {
        if (base.Collect())
        {
            CableHandler.Instance.DestroyCable(rigidbody2D);
            return true;
        }

        return false;
    }
}
