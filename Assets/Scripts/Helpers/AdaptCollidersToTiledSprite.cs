using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class AdaptCollidersToTiledSprite : MonoBehaviour
{
    [SerializeField] ColliderAdpationDirection direction;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] EdgeCollider2D edgeCollider2D;

    // Update is called once per frame
    void Update()
    {
        float sizeX = direction == ColliderAdpationDirection.Vertical ? spriteRenderer.size.x / 2 : Mathf.Round(spriteRenderer.size.x) / 2;
        float sizeY = direction == ColliderAdpationDirection.Horizontal ? spriteRenderer.size.y / 2 : Mathf.Round(spriteRenderer.size.y) / 2;
        spriteRenderer.size = new Vector2(sizeX * 2, sizeY * 2);
        transform.position = new Vector3(Mathf.Round(transform.position.x * 2) / 2f, Mathf.Round(transform.position.y * 2) / 2f, transform.position.z);

        if (edgeCollider2D != null)
            edgeCollider2D.points = new Vector2[] { new Vector2(-sizeX, 0), new Vector2(sizeX, 0) };

        if (boxCollider2D != null)
            boxCollider2D.size = new Vector2(sizeX * 2, sizeY * 2);

    }
    public enum ColliderAdpationDirection
    {
        Vertical,
        Horizontal,
        Both,
    }
}

