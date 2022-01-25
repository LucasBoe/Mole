using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicShadowDrawer : MonoBehaviour
{
    [SerializeField] AnimationCurve distanceDebugCurve;
    [SerializeField] int checkAmount360 = 45;
    [SerializeField] int detailScanDepth = 8;
    [SerializeField] float minSignificantDistance = 5f, minSignificantHitDifference = 2f;
    [SerializeField] int maxShadowDistance = 20;

    [SerializeField] int raycastCounterDebug = 0;

    Dictionary<float, AreaScanResult> scanData = new Dictionary<float, AreaScanResult>();

    private Dictionary<float, AreaScanResult> Do360Scan(int checks)
    {
        Dictionary<float, AreaScanResult> scan = new Dictionary<float, AreaScanResult>();
        for (int i = 0; i < checks; i++)
        {
            float angle = (i / (float)checks) * 360f;

            Vector2 hit = DoRaycast(transform.position, angle, maxShadowDistance) - (Vector2)transform.position;

            AreaScanResult result = new AreaScanResult { Hit = hit, Distance = Mathf.Abs(hit.magnitude) };

            scan.Add(angle, result);
        }

        return scan;
    }
    private Dictionary<float, AreaScanResult> DoDetailScan(Dictionary<float, AreaScanResult> scanData, List<AreaScanDetail> extremes)
    {
        foreach (AreaScanDetail detail in extremes)
        {
            float angle = Mathf.LerpAngle(detail.StartAngle, detail.EndAngle, 0.5f);
            Vector2 hit = DoRaycast(transform.position, angle, maxShadowDistance) - (Vector2)transform.position;
            if (!scanData.ContainsKey(angle))
                scanData.Add(angle, new AreaScanResult { Hit = hit, Distance = Mathf.Abs(hit.magnitude) });
        }

        return scanData;
    }

    // Update is called once per frame
    void Update()
    {
        raycastCounterDebug = 0;

        scanData.Clear();
        scanData = Do360Scan(checkAmount360);

        List<AreaScanDetail> extremes = new List<AreaScanDetail>();

        for (int i = 0; i < detailScanDepth; i++)
        {
            extremes = FetchExtremesByDistanceToOrigin(scanData);
            scanData = DoDetailScan(scanData, extremes);
        }

        //DrawDistanceCurve(scanData);
        UpdateMesh((scanData.OrderBy(scan => scan.Key).ToArray()).Select(foo => foo.Value.Hit).ToArray());
    }


    private List<AreaScanDetail> FetchExtremesByDistanceToOrigin(Dictionary<float, AreaScanResult> scanData)
    {
        List<AreaScanDetail> results = new List<AreaScanDetail>();
        AreaScanInformation before = null;

        foreach (KeyValuePair<float, AreaScanResult> current in scanData.OrderBy(a => a.Key))
        {
            if (before != null)
            {
                bool bothAir = before.Value.Distance > 19 && current.Value.Distance > 19;
                bool sharesXOrY = (before.Value.Hit.x == current.Value.Hit.x || before.Value.Hit.y == current.Value.Hit.y);
                float distanceDifference = Mathf.Abs(before.Value.Distance - current.Value.Distance);
                float hitDifference = Vector2.Distance(before.Value.Hit, current.Value.Hit);

                if (!bothAir && !sharesXOrY && (distanceDifference > minSignificantDistance || hitDifference > minSignificantHitDifference))
                {
                    AreaScanDetail detail = new AreaScanDetail();
                    detail.StartAngle = before.Angle;
                    detail.EndAngle = current.Key;
                    results.Add(detail);
                }
            }

            before = new AreaScanInformation() { Angle = current.Key, Value = current.Value };
        }

        return results;
    }


    private void UpdateMesh(Vector2[] vector2)
    {
        Mesh mesh;
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        mesh = GetComponent<MeshFilter>().mesh;

        for (int i = 0; i < vector2.Length; i++)
        {
            int index = i * 2;

            Vector2 fix = new Vector2(vector2[i].x, vector2[i].y);
            fix = fix.normalized * maxShadowDistance;
            newVertices.Add(new Vector2(fix.x, fix.y));
            newVertices.Add(new Vector2(vector2[i].x, vector2[i].y));

            if (index > 0 && index < vector2.Length * 2)
            {
                newTriangles.Add(index - 2);
                newTriangles.Add(index - 1);
                newTriangles.Add(index + 1);
                newTriangles.Add(index - 2);
                newTriangles.Add(index + 1);
                newTriangles.Add(index);
            }
        }

        newTriangles.Add(newVertices.Count - 2);
        newTriangles.Add(newVertices.Count - 1);
        newTriangles.Add(1);
        newTriangles.Add(newVertices.Count - 2);
        newTriangles.Add(1);
        newTriangles.Add(0);

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
    }

    Vector2 DoRaycast(Vector2 origin, float angle, float length, bool debug = false, int debugColorIndex = 0)
    {
        Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
        return DoRaycast(origin, dir, length, debug, debugColorIndex);
    }

    Vector2 DoRaycast(Vector2 origin, Vector2 dir, float length, bool debug = false, int debugColorIndex = 0)
    {
        raycastCounterDebug++;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, length, LayerMask.GetMask("Default"));
        if (hit)
        {
            if (debug)
                Debug.DrawLine(origin, hit.point, (debugColorIndex == 0) ? Color.red : Color.green);

            return hit.point;
        }
        else
        {
            if (debug)
                Debug.DrawLine(origin, origin + (dir * length), (debugColorIndex == 0) ? Color.blue : Color.yellow);
        }

        return origin + (dir * length);
    }
    //private void DebugDrawDistanceCurve(Dictionary<float, AreaScanResult> scanData)
    //{
    //    distanceDebugCurve = new AnimationCurve();
    //    foreach (KeyValuePair<float, AreaScanResult> scan in scanData)
    //    {
    //        distanceDebugCurve.AddKey(scan.Key, scan.Value.Distance);
    //    }
    //}
}
