using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionCheck
{
    public LayerMask LayerMask;
    public Vector2 Pos, Size = Vector2.one;
    public Color DebugColor;
    public bool IsDetecting => colliders != null && colliders.Length > 0;

    private Collider2D[] colliders;

    public CollisionCheck(float posX, float posY, float sizeX, float sizeY, LayerMask layerMask, Color debugColor)
    {
        Pos = new Vector2(posX, posY);
        Size = new Vector2(sizeX, sizeY);
        LayerMask = layerMask;
        DebugColor = debugColor;
    }

    public CollisionCheck(Vector2 pos, Vector2 size, LayerMask layerMask)
    {
        Pos = pos;
        Size = size;
        LayerMask = layerMask;
    }

    public void Update(Transform player)
    {
        colliders = Physics2D.OverlapBoxAll((Vector2)player.position + Pos, Size, 0, LayerMask);
    }

    //TODO: Remove and replace with T method below
    public IHangable[] GetHangables()
    {
        List<IHangable> hangables = new List<IHangable>();
        foreach (Collider2D collider in colliders)
        {
            var hangable = collider.GetComponent<IHangable>();
            if (hangable != null)
                hangables.Add(hangable);
        }

        return hangables.ToArray();
    }

    //TODO: Remove and replace with T method below
    public IFloor[] GetFloors()
    {
        List<IFloor> floors = new List<IFloor>();
        foreach (Collider2D collider in colliders)
        {
            var floor = collider.GetComponent<IFloor>();
            if (floor != null)
                floors.Add(floor);
        }

        return floors.ToArray();
    }

    internal T[] Get<T>()
    {
        List<T> elements = new List<T>();
        foreach (Collider2D collider in colliders)
        {
            if (collider != null)
            {
                var floor = collider.GetComponent<T>();
                if (floor != null)
                    elements.Add(floor);
            }
        }

        return elements.ToArray();
    }
}

public static class CollisionCheckUtil
{
    internal static string [] GetLineOfSightMask()
    {
        return new string[] { "Hangable", "HangableCollidable", "Default" };
    }
}
