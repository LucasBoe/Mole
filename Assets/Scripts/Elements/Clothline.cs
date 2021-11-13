using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothline : MonoBehaviour
{
    [SerializeField] HingeJoint2D start, end;
    [SerializeField] HingeJoint2D lineElementPrefab;
    [SerializeField] LineRenderer lineRenderer;

    Transform[] elements;

    // Start is called before the first frame update
    void Start()
    {
        elements = CreateLineElements();
    }

    private Transform[] CreateLineElements()
    {
        List<Transform> list = new List<Transform>();
        list.Add(start.transform);

        Vector2 startPos = start.transform.position;
        Vector2 endPos = end.transform.position;

        int distance = Mathf.FloorToInt(Vector2.Distance(startPos, endPos));
        int numberOfElements = distance + 1;
        HingeJoint2D before = start;

        for (int i = 0; i < numberOfElements; i++)
        {
            Vector2 pos = Vector2.Lerp(startPos, endPos, ((float)i / numberOfElements) + (1 / distance));
            HingeJoint2D newElement = Instantiate(lineElementPrefab, pos, Quaternion.identity, transform);
            before.connectedBody = newElement.GetComponent<Rigidbody2D>();
            before = newElement;
            list.Add(newElement.transform);
        }

        before.connectedBody = end.GetComponent<Rigidbody2D>();
        before.connectedAnchor = new Vector2(0, before.connectedAnchor.y);

        list.Add(end.transform);
        return list.ToArray();
    }

    private void Update()
    {
        if (elements != null && lineRenderer != null)
        {
            Debug.Log(elements != null);

            Vector3[] pos = new Vector3[elements.Length];

            for (int i = 0; i < elements.Length; i++)
                pos[i] = elements[i].position;

            lineRenderer.positionCount = elements.Length;
            lineRenderer.SetPositions(pos);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(start.transform.position, 0.25f);
        Gizmos.DrawSphere(end.transform.position, 0.25f);
        Gizmos.DrawLine(start.transform.position, end.transform.position);
    }
}
