using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeElement : CableElement
{
    [SerializeField] RopePhysicsSegment segmentPrefab;
    [SerializeField] FixedJoint2D target;
    [SerializeField] float debugLength;
    public FixedJoint2D Target => target;

    private List<RopePhysicsSegment> elements = new List<RopePhysicsSegment>();
    private RopePhysicsSegment Last => elements.Count == 0 ? null : elements[elements.Count - 1];

    public float DistanceToAttachedObject => Vector2.Distance(transform.position, attachJoint.connectedBody.position);

    private float length = 0;
    public float Length => length;

    internal void Setup(Rigidbody2D start, Rigidbody2D end, float length)
    {
        StartSetup(start, end, length);

        Vector2[] pos = new Vector2[Mathf.CeilToInt(length)];
        for (int i = 0; i < length; i++)
            pos[i] = Vector2.Lerp(start.position, end.position, 1f - (i / length));

        FinishSetup(length, pos);
    }
    public void Setup(Rigidbody2D start, Rigidbody2D end, Vector2[] travelPoints)
    {
        StartSetup(start, end, travelPoints.GetDistance());

        Vector2[] remapped = Util.RemapComplexLengthToPointsOfCertainDistance(new List<Vector3>(travelPoints.ToVector3Array()), 1);
        Vector2[] pos = new Vector2[Mathf.CeilToInt(length)];
        for (int i = 0; i < length; i++)
            pos[i] = remapped[(int)(((pos.Length - 1 - (float)i) / pos.Length) * remapped.Length)];

        FinishSetup(length, pos);
    }

    private void StartSetup(Rigidbody2D start, Rigidbody2D end, float length)
    {
        BasicSetup(start, end);
        this.length = length;
    }

    private void FinishSetup(float length, Vector2[] pos)
    {
        CreatePhysicsSegments(length, pos);
        visualizerInstance.Init(this);
    }

    private void CreatePhysicsSegments(float newLength, Vector2[] positions = null)
    {
        if (positions != null)
        {
            foreach (Vector2 item in positions)
            {
                Util.DebugDrawCross(item, Color.yellow, 0.6f, 4);
            }
        }

        int index = 0;
        while (newLength > 0)
        {
            bool previousElementExists = Last != null;
            Vector2 pos = (positions != null) ? positions[index] : (previousElementExists ? Last.transform.position.ToVector2() : transform.position.ToVector2());

            RopePhysicsSegment newElement = Instantiate(segmentPrefab, pos, Quaternion.identity, LayerHandler.Parent);
            Util.DebugDrawCircle(pos, Color.green, 0.5f, lifetime: 4);

            newElement.Connected(previousElementExists ? Last.Rigidbody : otherRigidbody);
            elements.Add(newElement);
            if (newLength >= 1)
                newLength--;
            else
            {
                newElement.SetDistance(newLength);
                newLength = 0;
            }

            index++;
        }
    }

    internal Vector2[] GetPoints()
    {
        return elements.Select(e => e.Rigidbody.position).ToArray();
    }

    public override void SetJointDistance(float newLength)
    {
        debugLength = newLength;

        if (newLength < length)
        {
            if (Mathf.Floor(length) > newLength)
                ModifyElements(newLength, ModifationDirection.Shorten);
            else
                SetLastElementLength(newLength - Mathf.Floor(newLength));
        }
        else
        {
            if (Mathf.Floor(newLength) >= length)
                ModifyElements(newLength, ModifationDirection.Lengthen);
            else
                SetLastElementLength(newLength - Mathf.Floor(newLength));
        }
        length = newLength;
    }

    private void ModifyElements(float newLengt, ModifationDirection direction)
    {
        if (direction == ModifationDirection.Lengthen)
        {
            float lastElementDifference = Mathf.Ceil(length) - length;
            SetLastElementLength(1);
            float rest = (newLengt - length) - lastElementDifference;
            CreatePhysicsSegments(rest);
        }
        else if (direction == ModifationDirection.Shorten)
        {
            int toRemove = Mathf.CeilToInt(length) - Mathf.CeilToInt(newLengt);
            while (toRemove > 0)
            {
                toRemove--;
                int index = elements.Count - 1;
                RopePhysicsSegment element = elements[index];
                elements.RemoveAt(index);
                Destroy(element.gameObject);
            }
            SetLastElementLength(newLengt - Mathf.Floor(newLengt));
        }
    }

    private void SetLastElementLength(float lastLength)
    {
        if (Last != null)
            Last.SetDistance(lastLength);
    }

    private void Update()
    {
        if (Last != null)
            target.connectedBody = Last.Rigidbody;
    }

    private void OnDestroy()
    {
        for (int i = elements.Count - 1; i >= 0; i--)
            Destroy(elements[i].gameObject);
    }

    private enum ModifationDirection
    {
        Shorten,
        Lengthen,
    }
    public override void Reconnect(Rigidbody2D to)
    {
        Debug.Log($"reconnected from {attachJoint.connectedBody.name} to {to.name}");
        attachJoint.connectedBody = to;
    }
}