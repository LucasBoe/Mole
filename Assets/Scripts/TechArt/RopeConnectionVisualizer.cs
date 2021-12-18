using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeConnectionVisualizer : MonoBehaviour
{
    [SerializeField] private Transform start, tween, end;
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

    internal void Init(Transform start, Transform end)
    {
        this.start = start;
        this.end = end;
    }

    // Update is called once per frame
    void Update()
    {
        if (!start || !end)
            return;

        float distance = Vector2.Distance(start.position, end.position);
        Vector2 center = (start.position + end.position) / 2f;
        Vector2 gravityModifier = Vector2.down * distance * gravityValue;
        Vector2 bufferModifier = Vector2.down * distance * 0.25f * buffer;
        tweenJoint.target = center + gravityModifier + bufferModifier;

        Vector2[] points = new Vector2[] { start.position, tween.position, end.position };

        for (int i = 0; i < distance / 10f + 1; i++)
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
