using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class AdaptEdgeColliderToTiledSprite : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        EdgeCollider2D edgeCollider2D = GetComponent<EdgeCollider2D>();

        float size = Mathf.Round(spriteRenderer.size.x)/2;
        spriteRenderer.size = new Vector2(size * 2, spriteRenderer.size.y);
        transform.position = new Vector3(Mathf.Round(transform.position.x * 2) / 2f, transform.position.y, transform.position.z);

        edgeCollider2D.points = new Vector2[] { new Vector2(-size, 0), new Vector2(size, 0) };
    }
}
