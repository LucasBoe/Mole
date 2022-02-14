using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    [SerializeField] Rigidbody2D dynamicStart, staticEnd;
    [SerializeField] List<RopeAnchor> anchors = new List<RopeAnchor>();
    [SerializeField] float freezeTime = 3;

    private void Start()
    {
        if (anchors.Count > 0)
            RopeHandler.Instance.CreateRope(dynamicStart, staticEnd, CreateRopePoints());

        if (freezeTime > 0)
        {
            FreezeObjects(true);
            StartCoroutine(DelayRoutine(freezeTime, () =>
            {
                FreezeObjects(false);
                Destroy(gameObject);
            }));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Vector2[] CreateRopePoints()
    {
        List<Vector3> points = new List<Vector3>();
        if (dynamicStart != null) points.Add(dynamicStart.position);

        for (int i = 0; i < anchors.Count; i++)
        {
            RopeAnchor anchor = anchors[i];
            Vector2 inVector = GetDirVector((i == 0 ? dynamicStart.position : anchors[i - 1].transform.position.ToVector2()), anchor.transform.position.ToVector2());
            Vector2 outVector = GetDirVector(anchor.transform.position.ToVector2(), (i == anchors.Count - 1 ? staticEnd.position : anchors[i + 1].transform.position.ToVector2()));
            points.Add(anchor.GetAroundPosition(inVector));
            points.Add(anchor.GetAroundPosition(outVector));
        }

        if (staticEnd != null) points.Add(staticEnd.position);

        return Util.RemapComplexLengthToPointsOfCertainDistance(points, 1f);
    }

    private void FreezeObjects(bool freeze)
    {
        RigidbodyConstraints2D constraints2D = freeze ? RigidbodyConstraints2D.FreezePosition : RigidbodyConstraints2D.None;
        dynamicStart.constraints = constraints2D;
        staticEnd.constraints = constraints2D;
    }

    private void OnDrawGizmos()
    {
        List<Vector3> points = new List<Vector3>();
        if (dynamicStart != null) points.Add(dynamicStart.position);

        for (int i = 0; i < anchors.Count; i++)
        {
            RopeAnchor anchor = anchors[i];
            Vector2 inVector = GetDirVector((i == 0 ? dynamicStart.position : anchors[i - 1].transform.position.ToVector2()), anchor.transform.position.ToVector2());
            Vector2 outVector = GetDirVector(anchor.transform.position.ToVector2(), (i == anchors.Count - 1 ? staticEnd.position : anchors[i + 1].transform.position.ToVector2()));
            points.Add(anchor.GetAroundPosition(inVector));
            points.Add(anchor.GetAroundPosition(outVector));
        }

        //points.AddRange(anchors.Select(a => a.transform.position));
        if (staticEnd != null) points.Add(staticEnd.position);

        Gizmos.color = Color.yellow;

        for (int i = 1; i < points.Count; i++)
            Gizmos.DrawLine(points[i - 1], points[i]);

        foreach (RopeAnchor anchor in anchors)
            Gizmos.DrawWireSphere(anchor.transform.position, 0.5f);

        //Vector2[] remapped = Util.RemapLengthToPointsOfCertainDistance(points, 1f);
        Vector2[] remapped = Util.RemapComplexLengthToPointsOfCertainDistance(points, 1f);

        Gizmos.color = Color.red;

        foreach (Vector2 point in remapped)
            Gizmos.DrawWireSphere(point, 0.2f);

    }

    private Vector2 GetDirVector(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    public IEnumerator DelayRoutine(float delay, System.Action executeOnFinish)
    {
        yield return new WaitForSeconds(delay);
        executeOnFinish?.Invoke();
    }
}
