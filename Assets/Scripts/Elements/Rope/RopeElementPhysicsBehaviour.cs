using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeElementPhysicsBehaviour : MonoBehaviour
{
    [SerializeField] RopePhysicsSegment segmentPrefab;
    [SerializeField] FixedJoint2D target;
    public FixedJoint2D Target => target;

    private List<RopePhysicsSegment> elements = new List<RopePhysicsSegment>();
    private RopePhysicsSegment Last => elements.Count == 0 ? null : elements[elements.Count - 1];


    private Rigidbody2D connectedBody;
    private float length = 0;
    public float Length => length;

    public void Init(Rigidbody2D endBody, Rigidbody2D startBody, Vector2[] travelPoints)
    {
        connectedBody = startBody;

        length = travelPoints.GetDistance();
        Vector2[] pos = new Vector2[Mathf.CeilToInt(length)];
        for (int i = 0; i < length; i++)
            pos[i] = travelPoints[(int)(((pos.Length - 1 - (float)i) / pos.Length) * travelPoints.Length)];

        CreateRopeElements(length, pos);
    }
    internal void Init(Rigidbody2D endBody, Rigidbody2D startBody)
    {
        connectedBody = startBody;
        length = Vector2.Distance(endBody.position, startBody.position);
        //Vector2[] pos = new Vector2[Mathf.CeilToInt(length)];
        //for (int i = 0; i < length; i++)
        //    pos[i] = Vector2.Lerp(startBody.position, endBody.position, i / length);

        CreateRopeElements(length);
    }

    private void CreateRopeElements(float newLength, Vector2[] positions = null)
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

            newElement.Connected(previousElementExists ? Last.Rigidbody : connectedBody);
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

    public void SetLength(float newLength)
    {
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
            CreateRopeElements(rest);
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
}