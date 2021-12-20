using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Rope
{
    private float length;
    public float Length { get => length; }

    private List<RopeAnchor> anchors;
    public bool HasAnchors => anchors != null && anchors.Count > 0;

    private RopeElement[] elements = new RopeElement[2];
    private RopeElement one => elements[0];
    private RopeElement two => elements[1];

    private float distribution = 0.5f;
    private float deadLength = 0;

    private float smoothForceDifference = 0;
    private float smoothDistanceDifference = 0;

    public Rope(Rigidbody2D start, RopeAnchor[] anchors, Rigidbody2D end)
    {
        this.anchors = new List<RopeAnchor>(anchors);
        elements[0] = RopeHandler.Instance.CreateRopeElement(start, anchors[0].Rigidbody2D);
        elements[1] = RopeHandler.Instance.CreateRopeElement(end, anchors[anchors.Length - 1].Rigidbody2D);

        RecalulateLengthAndDistributionFromDistance();
    }

    public void Update()
    {
        float distributionChange = (BalanceOperationn() / length);
        distribution += distributionChange;
        distribution = Mathf.Clamp(distribution, 0, 1);

        one.SetJointDistance((length - deadLength) * distribution);
        two.SetJointDistance((length - deadLength) * (1f - distribution));
        one.Rigidbody2D.AddForce(distributionChange < 0 ? Vector2.down : Vector2.up);
        two.Rigidbody2D.AddForce(distributionChange > 0 ? Vector2.down : Vector2.up);
    }

    private float BalanceOperationn()
    {
        float forceDifference = (one.PullForce - two.PullForce) * Time.deltaTime;
        float distanceDifference = Mathf.Abs(one.DistanceDifference + two.DistanceDifference);

        smoothForceDifference = Mathf.Lerp(smoothForceDifference, forceDifference, Time.deltaTime);
        smoothDistanceDifference = Mathf.Lerp(smoothDistanceDifference, distanceDifference, Time.deltaTime);

        float decreasedByDistance = Mathf.Max(1 - smoothDistanceDifference, 0) * smoothForceDifference;
        Debug.LogWarning($"Run Balance Operation: {smoothForceDifference}");
        return smoothForceDifference;
    }

    private void RecalulateLengthAndDistributionFromDistance()
    {
        Vector2[] points = GetPointsFromRigidbodys();

        float startLength = 0;
        float endLength = 0;
        deadLength = 0;

        for (int i = 1; i < points.Length; i++)
        {
            float l = Vector2.Distance(points[i - 1], points[i]);
            if (i == 1)
                startLength = l;
            else if (i == points.Length - 1)
                endLength = l;
            else
                deadLength += l;
        }

        distribution = startLength / (startLength + endLength);
        length = startLength + deadLength + endLength;
    }

    private Vector2[] GetPointsFromRigidbodys()
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(elements[0].Rigidbody2D.position);
        points.AddRange(anchors.Select(a => a.Rigidbody2D.position));
        points.Add(elements[1].TargetRigidbody2D.position);

        return points.ToArray();
    }
}
