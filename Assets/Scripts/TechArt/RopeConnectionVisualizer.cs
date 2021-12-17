using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeConnectionVisualizer : MonoBehaviour
{
    [SerializeField] Transform start, tween, end;
    TargetJoint2D tweenJoint;
    LineRenderer lineRenderer;

    [SerializeField] float smoothValue = 0.25f;
    [SerializeField] float gravityValue = 0.2f;

    void Start()
    {
        tweenJoint = GetComponentInChildren<TargetJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(start.position, end.position);
        Vector2 center = (start.position + end.position) / 2f;
        Vector2 gravityModifier = Vector2.down * distance * gravityValue;
        tweenJoint.target = center + gravityModifier;

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
}
