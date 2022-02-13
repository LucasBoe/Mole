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
            RopeHandler.Instance.CreateRope(dynamicStart, anchors.ToArray(), staticEnd);
        else
            RopeHandler.Instance.CreateRope(dynamicStart, new RopeAnchor[0], staticEnd);

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
        points.AddRange(anchors.Select(a => a.transform.position));
        if (staticEnd != null) points.Add(staticEnd.position);

        Gizmos.color = Color.yellow;

        for (int i = 1; i < points.Count; i++)
            Gizmos.DrawLine(points[i - 1], points[i]);

        foreach (RopeAnchor anchor in anchors)
            Gizmos.DrawWireSphere(anchor.transform.position, 0.5f);
    }

    public IEnumerator DelayRoutine(float delay, System.Action executeOnFinish)
    {
        yield return new WaitForSeconds(delay);
        executeOnFinish?.Invoke();
    }
}
