using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Stairs))]
[CanEditMultipleObjects]
public class StairsEditor : Editor
{
    Stairs stairs;
    private void OnEnable()
    {
        stairs = target as Stairs;
        foreach (Transform child in stairs.transform)
        {
            child.hideFlags = HideFlags.None;
        }

        stairs.otherTarget.transform.hideFlags = HideFlags.HideInHierarchy;
        stairs.spriteHolder.transform.hideFlags = HideFlags.HideInHierarchy;
    }

    private void OnSceneGUI()
    {

        if (stairs == null) return;

        Vector3 posBefore = stairs.otherTarget.position;
        Handles.color = Color.cyan;
        Handles.DrawLine(stairs.transform.position, posBefore, 4f);

        EditorGUI.BeginChangeCheck();

        Vector3 end = ToPossiblePosition(Handles.FreeMoveHandle(posBefore, Quaternion.identity, 0.5f, Vector3.one, Handles.SphereHandleCap), stairs.transform.position); ;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(stairs.otherTarget, "Changed Look Target");
            stairs.otherTarget.position = end;

            Vector3 start = stairs.transform.position;

            UpdateStairVisuals(start, end, stairs.spriteHolder);
            UdateStatirCollider(Vector2.zero, end - start);
        }

        if (stairs.transform.hasChanged)
            stairs.transform.position = stairs.transform.position.Rounded();
    }

    private void UdateStatirCollider(Vector2 start, Vector2 end)
    {
        List<Vector2> points = new List<Vector2>();

        float forward = (end.x > start.x) ? 1 : -1;

        points.Add(start);
        points.Add(start - Vector2.right * forward);
        points.Add(start - Vector2.right * forward + Vector2.up * 1.5f);
        points.Add(start + Vector2.up * 1.5f);
        points.Add(end + Vector2.up * 1.5f);
        points.Add(end + Vector2.up * 1.5f + Vector2.right * forward);
        points.Add(end + Vector2.right * forward);
        points.Add(end);

        stairs.trigger.points = points.ToArray();
    }

    private Vector3 ToPossiblePosition(Vector3 toRound, Vector3 start)
    {
        bool right = toRound.x > start.x;
        bool up = toRound.y > start.y;

        int xSize = CalculateXSize(start, toRound.Rounded());

        return new Vector2(start.x + xSize * (right ? 1 : -1), start.y + xSize * (up ? 1 : -1));
    }

    private void UpdateStairVisuals(Vector3 start, Vector3 end, Transform spriteHolder)
    {
        spriteHolder.DestroyAllChildrenImmediate();
        int spriteCount = CalculateXSize(start, end);

        bool right = end.x > start.x;
        bool up = end.y > start.y;

        for (int i = 0; i < spriteCount; i++)
        {
            Vector3 point = Vector3.Lerp(start, end, (float)i / spriteCount) + (up ? Vector3.zero : new Vector3 (1,-1));

            GameObject newChild = new GameObject("stair_auto");
            newChild.transform.position = point;
            newChild.transform.parent = spriteHolder;

            SpriteRenderer sprite = newChild.AddComponent<SpriteRenderer>();
            sprite.sprite = stairs.sprite45;
            sprite.flipX = !right == up;
            sprite.sortingLayerID = stairs.SortingLayerID;
        }
    }

    private static int CalculateXSize(Vector3 start, Vector3 end)
    {
        return Mathf.RoundToInt(Mathf.Abs(start.x - end.x));
    }
}
