using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopePhysicsBehaviour : MonoBehaviour
{
    [SerializeField] HingeJoint2D hinge;
    [SerializeField] RopePhysicsElement elementPrefab;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] TargetJoint2D target;
    public TargetJoint2D Target => target;

    private List<RopePhysicsElement> elements = new List<RopePhysicsElement>();
    private RopePhysicsElement Last => elements.Count == 0 ? null : elements[elements.Count - 1];
    private float length = 0;

    public void Init(float startLength)
    {
        length = startLength;
        CreateRopeElements(length);
    }

    private void CreateRopeElements(float newLength)
    {
        while (newLength > 0)
        {
            bool previousElementExists = Last != null;
            RopePhysicsElement newElement = Instantiate(elementPrefab, previousElementExists ? Last.transform.position : transform.position, Quaternion.identity);

            newElement.Connected(previousElementExists ? Last.Rigidbody : GetComponent<Rigidbody2D>());
            elements.Add(newElement);
            if (newLength >= 1)
                newLength--;
            else
            {
                newElement.SetDistance(newLength);
                newLength = 0;
            }
        }
    }
    private void SetLength(float newLength)
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
                RopePhysicsElement element = elements[index];
                elements.RemoveAt(index);
                Destroy(element.gameObject);
            }
            SetLastElementLength(newLengt - Mathf.Floor(newLengt));
        }
    }

    private void SetLastElementLength(float lastLength)
    {
        Last.SetDistance(lastLength);
    }

    private void Update()
    {
        lineRenderer.positionCount = elements.Count;
        lineRenderer.SetPositions(elements.Select(e => e.Rigidbody.position).ToArray().ToVector3Array());
        target.target = Last.GetEnd();
    }

    private enum ModifationDirection
    {
        Shorten,
        Lengthen,
    }
}