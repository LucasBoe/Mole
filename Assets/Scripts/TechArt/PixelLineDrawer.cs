using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelLineDrawer : SingletonBehaviour<PixelLineDrawer>
{
    public static Camera main;

    internal void Register(PixelLine line)
    {
        lines.Add(line);
    }

    internal void Unregister(PixelLine line)
    {
        lines.Remove(line);
    }

    SpriteRenderer renderer;
    Texture2D texture;

    public const int sizeX = 160;
    public const int sizeY = 90;

    List<PixelLine> lines = new List<PixelLine>();

    void Start()
    {
        main = Camera.main;
        renderer = GetComponentInChildren<SpriteRenderer>();

        texture = new Texture2D(sizeX, sizeY);
        Sprite mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, sizeX, sizeY), new Vector2(0.5f, 0.5f), 100.0f);
        texture.filterMode = FilterMode.Point;
        renderer.sprite = mySprite;

        ClearTexture();
    }

    private void ClearTexture()
    {
        Color trans = new Color(0, 0, 0, 0);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, trans);
            }
        }
        texture.Apply();
    }

    private void FixedUpdate()
    {
        ClearTexture();

        foreach (PixelLine line in lines)
        {

            List<Vector2Int> pointBuffer = new List<Vector2Int>();
            foreach (var item in line.GetSegments())
            {
                Vector2 start = item.Start;
                Vector2 end = item.End;

                if (!OutOfBounds(start) && !OutOfBounds(end))
                {
                    int distance = (int)Vector2.Distance(start, end);
                    for (int i = 0; i < distance; i++)
                    {
                        Vector2 point = Vector2.Lerp(start, end, (float)i / distance);
                        Vector2Int pointInt = new Vector2Int((int)point.x, (int)point.y);
                        pointBuffer.Add(pointInt);
                    }
                }
            }

            foreach (var item in pointBuffer)
            {
                texture.SetPixel(item.x, item.y, Color.white);
            }
        }

        texture.Apply();
    }

    private bool OutOfBounds(Vector2 pos)
    {
        return pos.x < sizeX && pos.y < sizeY && pos.x > 0 && pos.y > 0;
    }

    private void LateUpdate()
    {
        float x = ((main.transform.position.x * 8f) % 8f) / 8f;
        float y = ((main.transform.position.y * 8f) % 8f) / 8f;

        renderer.transform.localPosition = new Vector3(x / sizeX, y / sizeY, 1);
    }
}

public class PixelLineSegment
{
    public Vector3 WorldStart;
    public Vector3 WorldEnd;
    public Vector2 Start => TranformToTextureSpace(WorldStart);
    public Vector2 End => TranformToTextureSpace(WorldEnd);

    private Vector2 TranformToTextureSpace(Vector3 worldStart)
    {
        Vector3 toLocal = worldStart - PixelLineDrawer.main.transform.position;
        return new Vector2(toLocal.x * 8 - PixelLineDrawer.sizeX / 2, toLocal.y * 8 - PixelLineDrawer.sizeY / 2);
    }

}

[System.Serializable]
public class PixelLine
{
    public Vector3[] Points;

    public PixelLineSegment[] GetSegments()
    {
        List<PixelLineSegment> segments = new List<PixelLineSegment>();

        for (int i = 1; i < Points.Length; i++)
        {
            PixelLineSegment segment = new PixelLineSegment();
            segment.WorldStart = Points[i - 1];
            segment.WorldEnd = Points[i];
            segments.Add(segment);
        }

        return segments.ToArray();
    }
}
