using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeElementVisualizer : MonoBehaviour
{
    [SerializeField] RopeElementPhysicsBehaviour physicsBehaviour;
    [SerializeField] private Rigidbody2D start, end;
    [SerializeField] private Transform tween;
    private TargetJoint2D tweenJoint;
    private LineRenderer lineRenderer;

    [SerializeField] private float smoothValue = 0.25f;
    [SerializeField] private float gravityValue = 0.2f;

    [SerializeField] private float buffer;

    void Start()
    {
        tweenJoint = GetComponentInChildren<TargetJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    internal void Init(Rigidbody2D start, Rigidbody2D end, RopeElementPhysicsBehaviour physicsBehaviour = null)
    {
        this.start = start;
        this.end = end;
        this.physicsBehaviour = physicsBehaviour;
        tween.position = (start.position + end.position) / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!physicsBehaviour && (!start || !end))
            return;

        Vector2[] points = new Vector2[] { start.position, tween.position, end.position };
        float distance = Vector2.Distance(start.position, end.position);

        if (physicsBehaviour != null)
        {
            points = physicsBehaviour.GetPoints();
            distance = physicsBehaviour.Length;
        }

        for (int i = 0; i < distance / Mathf.Pow(points.Length + 1, 2) ; i++)
        {
            points = Util.SmoothToCurve(points, smoothValue);
        }

        foreach (Vector2 point in points)
        {
            Util.DebugDrawCircle(point, Color.yellow, 0.5f);
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points.ToVector3Array());
    }

    internal void SetBuffer(float buffer)
    {
        this.buffer = buffer;
    }
}
