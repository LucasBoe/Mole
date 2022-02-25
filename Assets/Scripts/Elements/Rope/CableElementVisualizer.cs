using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CableElementVisualizer : MonoBehaviour
{
    [SerializeField] RopeElement ropeElement;
    [SerializeField] private Rigidbody2D start, end;
    [SerializeField] private Transform tween;
    private LineRenderer lineRenderer;

    [SerializeField] private float smoothValue = 0.25f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(RopeElement ropeElement)
    {
        this.ropeElement = ropeElement;
    }

    public void Init(Rigidbody2D start, Rigidbody2D end, RopeElement physicsBehaviour = null)
    {
        this.start = start;
        this.end = end;
    }

    // Update is called once per frame
    void Update()
    {
        if (ropeElement)
        {
            Vector2[] points = ropeElement.GetPoints();
            float distance = ropeElement.Length;
            DisplayPoints(points, distance);
        }
        else if (start && end)
        {
            Vector2[] points = new Vector2[] { start.position, end.position };
            float distance = Vector2.Distance(start.position, end.position);
            DisplayPoints(points, distance);
        }
    }

    private void DisplayPoints(Vector2[] points, float distance)
    {
        for (int i = 0; i < distance / Mathf.Pow(points.Length + 1, 2); i++)
        {
            points = Util.SmoothToCurve(points, smoothValue);
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points.ToVector3Array());
    }
}
