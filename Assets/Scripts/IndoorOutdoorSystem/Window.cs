using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField] Sprite insideSprite, outsideSprite;
    bool isAbove;
    SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            isAbove = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            isAbove = false;
    }

    private void Update()
    {
        if (isAbove && PlayerInputHandler.PlayerInput.Interact)
        {
            isAbove = false;
            bool isInside = OutdoorIndoorHandler.Instance.Switch();
            spriteRenderer.sprite = isInside ? insideSprite : outsideSprite;
        }
    }
}
