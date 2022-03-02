using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSpriteCreator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider2D;
    private Sprite mySprite;

    // Start is called before the first frame update
    void Start()
    {
        Bounds bounds = collider2D.bounds;
        Vector2 size = bounds.max - bounds.min;
        Vector2 relativeOffset = (transform.position - bounds.min) / size;
        Debug.Log($"size = { size }");
        var texture = new Texture2D(Mathf.RoundToInt(size.x * 8), Mathf.RoundToInt(size.y * 8), TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                bool white = collider2D.OverlapPoint(bounds.min + new Vector3(x / 8f, y / 8f, 0));
                texture.SetPixel(x, y, white ? Color.white : Color.clear);
            }
        }
        texture.Apply();
        mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), relativeOffset, 8.0f);
        spriteRenderer.sprite = mySprite;
    }
}
