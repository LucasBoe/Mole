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
    [SerializeField] private RopeElementVisualizer ropeElementVisualizerPrefab;

    private States current = States.Throw;
    private Vector2 startPos;
    private float lastDistance;
    private RopeElementVisualizer visualizer;
    private List<Vector2> travelPositions = new List<Vector2>();

    private void Start()
    {
        startPos = transform.position;
        travelPositions.Add(startPos);
        visualizer = Instantiate(ropeElementVisualizerPrefab);
        visualizer.Init(PlayerController.Context.Rigidbody, rigidbody2D);
        Time.timeScale = 0.33f;
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
        }
    }

    private void FixateHook()
    {
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        collider.enabled = false;
        travelPositions.Add(transform.position);
        RopeHandler.Instance.CreateRope(PlayerController.Context.Rigidbody, rigidbody2D, travelPositions.ToArray());
        Destroy(visualizer.gameObject);
    }

    public override void Collect()
    {
        RopeHandler.Instance.DestroyRope(rigidbody2D);
        base.Collect();
    }
}
