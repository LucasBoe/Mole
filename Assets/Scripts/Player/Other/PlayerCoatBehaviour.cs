using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoatBehaviour : MonoBehaviour
{
    [SerializeField] Transform coatBase;
    [SerializeField] Rigidbody2D[] coatElements;
    LineRenderer lineRenderer;
    [SerializeField] bool KeepMaxDistance = false;
    [SerializeField] float MaxDistanceToKeep = 1f;

    private void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = coatElements.Length;
    }

    private void LateUpdate()
    {
        Vector2 before = coatBase.position;
        for (int i = 0; i < coatElements.Length; i++)
        {
            Rigidbody2D element = coatElements[i];
            if (KeepMaxDistance)
            {
                float distanceToBefore = Vector2.Distance(before, element.position);
                if (distanceToBefore > MaxDistanceToKeep)
                    element.MovePosition(Vector2.MoveTowards(element.position, before, Mathf.Max(0, distanceToBefore - MaxDistanceToKeep)));
            }
            lineRenderer.SetPosition(i, element.position);
        }
    }
}
