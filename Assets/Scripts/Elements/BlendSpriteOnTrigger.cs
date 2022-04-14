using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendSpriteOnTrigger : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            spriteRenderer.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            spriteRenderer.enabled = true;
    }
}
