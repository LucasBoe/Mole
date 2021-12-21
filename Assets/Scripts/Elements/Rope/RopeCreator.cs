using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    [SerializeField] Rigidbody2D start, end;
    [SerializeField] List<RopeAnchor> anchors = new List<RopeAnchor>();

    private void Start()
    {
        if (anchors.Count > 0)
            RopeHandler.Instance.CreateRope(start, anchors.ToArray(), end);
        else
            RopeHandler.Instance.CreateRope(start, new RopeAnchor[0] ,end);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        List<Vector3> points = new List<Vector3>();
        if (start != null) points.Add(start.position);
        points.AddRange(anchors.Select(a => a.transform.position));
        if (end != null) points.Add(end.position);

        Gizmos.color = Color.yellow;

        for (int i = 1; i < points.Count; i++)
            Gizmos.DrawLine(points[i - 1], points[i]);

        foreach (RopeAnchor anchor in anchors)
            Gizmos.DrawWireSphere(anchor.transform.position, 0.5f);
    }
}
