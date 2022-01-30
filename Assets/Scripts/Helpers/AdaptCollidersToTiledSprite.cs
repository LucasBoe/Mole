using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class AdaptCollidersToTiledSprite : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] EdgeCollider2D edgeCollider2D;

    // Update is called once per frame
    void Update()
    {
        float sizeX = Mathf.Round(spriteRenderer.size.x) / 2;
        spriteRenderer.size = new Vector2(sizeX * 2, spriteRenderer.size.y);
        transform.position = new Vector3(Mathf.Round(transform.position.x * 2) / 2f, transform.position.y, transform.position.z);

        if (edgeCollider2D != null)
            edgeCollider2D.points = new Vector2[] { new Vector2(-sizeX, 0), new Vector2(sizeX, 0) };

        if (boxCollider2D != null)
            boxCollider2D.size = new Vector2(sizeX * 2, boxCollider2D.size.y);

    }
}
