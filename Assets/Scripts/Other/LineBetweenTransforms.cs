using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineBetweenTransforms : MonoBehaviour
{
    [SerializeField] Transform transform1, transform2;
    [SerializeField] Vector2 offset1, offset2;
    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, (Vector2)transform1.position + offset1);
        lineRenderer.SetPosition(1, (Vector2)transform2.position + offset2);
    }
}
