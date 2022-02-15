using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeHook : CollectablePlayerItem
{
    public enum States
    {
        Throw,
        MoveBack,
        Static,
    }

    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private Collider2D collider;
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
                travelPositions.Add(transform.position);

            if (Physics2D.Raycast(new Vector2(transform.position.x - 0.1f, transform.position.y + 0.2f), Vector2.down, 0.5f, LayerMask.GetMask("Default"))
                || Physics2D.Raycast(new Vector2(transform.position.x + 0.1f, transform.position.y + 0.2f), Vector2.down, 0.5f, LayerMask.GetMask("Default")))
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
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = Vector2.Lerp(PlayerController.Instance.transform.position, path[i], (float)i / Mathf.Max(length - 1, 1));
        }

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

    public override void Collect()
    {
        CableHandler.Instance.DestroyCable(rigidbody2D);
        base.Collect();
    }
}
