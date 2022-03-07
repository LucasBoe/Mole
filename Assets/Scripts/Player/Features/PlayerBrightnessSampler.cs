using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerBrightnessSampler : PlayerBehaviour
{
    [SerializeField] RenderTexture gameTexture;
    [SerializeField] Vector2[] sampleLocations;
    [SerializeField] [Range(0, 1)] float avgBrightness = 0.5f;
    [SerializeField] SampleData[] sampleData;

    public static System.Action<float> OnSampleNewPlayerBrightness;

    private void Start()
    {
        sampleData = new SampleData[sampleLocations.Length];
    }

    private void FixedUpdate()
    {
        if (gameTexture == null)
            return;   

        avgBrightness = Mathf.Lerp(avgBrightness, SampleBrightness(ReadPixels(sampleLocations)), 0.05f);
        OnSampleNewPlayerBrightness?.Invoke(avgBrightness);
    }

    private float SampleBrightness(Color[] color)
    {
        float b = 0;
        float max = 0;

        for (int i = 0; i < color.Length; i++)
        {
            Color c = color[i];
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            sampleData[i].Color = c;
            sampleData[i].Brightness = v;
            max = Mathf.Max(max, v);
            b += v;
        }  

        return Mathf.Lerp(max, b / color.Length, 0.5f);
    }

    private Color[] ReadPixels(Vector2[] localPos)
    {
        List<Color> samples = new List<Color>();

        //Create Realtime Texture
        RenderTexture.active = gameTexture;
        Texture2D tex = new Texture2D(gameTexture.width, gameTexture.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

        foreach (Vector2 pos in localPos)
        {
            samples.Add(SampleColorFromWorldPoint(pos, tex));
        }

        return samples.ToArray();
    }

    private Color SampleColorFromWorldPoint(Vector2 localPos, Texture2D tex)
    {
        Vector2 pixelPos = CameraController.WorldToScreenPoint(transform.TransformPoint(localPos)) / new Vector2(Screen.width, Screen.height);
        Color c = tex.GetPixelBilinear(pixelPos.x, pixelPos.y);
        return c;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Vector2 loc in sampleLocations)
        {
            Vector3 world = transform.TransformPoint(loc);
            Gizmos.DrawWireSphere(world, 0.1f);
        }
    }
}

[System.Serializable]
public struct SampleData
{
    public Color Color;
    [Range(0, 1)]
    public float Brightness;
}
