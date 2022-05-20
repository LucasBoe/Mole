using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOutlineRenderer : MonoBehaviour
{
    [SerializeField] Shader outlineShader;
    [SerializeField] protected Color OutlineColor;
    SpriteRenderer spriteRenderer;
    Material outlineMat;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        outlineMat = new Material(outlineShader);
        spriteRenderer.materials = new Material[] { spriteRenderer.material, outlineMat };
    }

    public void SetOutline(Color color)
    {
        outlineMat.SetColor("outlineColor", color);
    }

    [ContextMenu("SetColor")]
    private void SetColor()
    {
        SetOutline(OutlineColor);
    }
}
