using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicViewAreaDetector : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    const int checks = 360;

    [SerializeField] int toDraw;
    [SerializeField] bool switchToDraw;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = checks;
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector4> lines = new List<Vector4>();

        int mask = LayerMask.GetMask("Default");

        for (int i = 0; i < checks; i++)
        {
            float angle = (i / (float)checks) * 360f;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

            Vector2 outer = (Vector2)transform.position + (dir * 20) - (Vector2)transform.position;
            Vector2 hit = DoRaycast(transform.position, dir, 20, mask) - (Vector2)transform.position; ;

            lines.Add(new Vector4(outer.x, outer.y, hit.x, hit.y));
            //lineRenderer.SetPosition(i, hit);
        }

        UpdateMesh(lines.ToArray());
    }

    private void UpdateMesh(Vector4[] vector4)
    {
        Mesh mesh;
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        mesh = GetComponent<MeshFilter>().mesh;

        for (int i = 0; i < vector4.Length; i++)
        {
            int index = i * 2;

            newVertices.Add(new Vector2(vector4[i].x, vector4[i].y));
            newVertices.Add(new Vector2(vector4[i].z, vector4[i].w));

            if (index > 1 && index == toDraw)
            {
                if (switchToDraw)
                    lineRenderer.SetPositions(new Vector3[] { newVertices[toDraw - 2], newVertices[toDraw - 1], newVertices[toDraw + 1] });
                else
                    lineRenderer.SetPositions(new Vector3[] { newVertices[toDraw - 2], newVertices[toDraw + 1], newVertices[toDraw] });
            }

            if (index > 0 && index < vector4.Length * 2)
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

    Vector2 DoRaycast(Vector2 origin, Vector2 dir, float length, int mask)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, length, mask);
        if (hit == true)
        {
            return hit.point;
        }

        return origin + (dir * length);
    }
}
