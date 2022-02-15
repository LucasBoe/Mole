using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CableCreator : MonoBehaviour
{
    public enum Modes
    {
        Chain,
        Rope
    }

    [SerializeField] private Modes mode;
    [SerializeField] private Rigidbody2D start, end;
    [SerializeField] private List<CableAnchor> anchors = new List<CableAnchor>();
    [SerializeField] private float freezeTime = 3;

    private void Start()
    {
        //if (anchors.Count > 0)
        //    CableHandler.Instance.CreateCable(start, end, CreateRopePoints());

        if (mode == Modes.Chain)
        {
            CableHandler.Instance.CreateChain(start, end, anchors);
        } else
        {
            CableHandler.Instance.CreateRope(start, end, anchors);
        }

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
        if (start != null) points.Add(start.position);

        for (int i = 0; i < anchors.Count; i++)
        {
            CableAnchor anchor = anchors[i];
            Vector2 inVector = GetDirVector((i == 0 ? start.position : anchors[i - 1].transform.position.ToVector2()), anchor.transform.position.ToVector2());
            Vector2 outVector = GetDirVector(anchor.transform.position.ToVector2(), (i == anchors.Count - 1 ? end.position : anchors[i + 1].transform.position.ToVector2()));
            points.Add(anchor.GetAroundPosition(inVector));
            points.Add(anchor.GetAroundPosition(outVector));
        }

        if (end != null) points.Add(end.position);

        return Util.RemapComplexLengthToPointsOfCertainDistance(points, 1f);
    }

    private void FreezeObjects(bool freeze)
    {
        RigidbodyConstraints2D constraints2D = freeze ? RigidbodyConstraints2D.FreezePosition : RigidbodyConstraints2D.None;
        start.constraints = constraints2D;
        end.constraints = constraints2D;
    }

    private void OnDrawGizmos()
    {
        List<Vector3> points = new List<Vector3>();

        if (start != null) points.Add(start.position);
        points.AddRange(anchors.Select(a => a.transform.position));
        if (end != null) points.Add(end.position);

        Gizmos.color = mode == Modes.Rope ? Color.yellow : Color.gray;

        for (int i = 1; i < points.Count; i++)
            Gizmos.DrawLine(points[i - 1], points[i]);

        foreach (CableAnchor anchor in anchors)
            Gizmos.DrawWireSphere(anchor.transform.position, 0.5f);
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
