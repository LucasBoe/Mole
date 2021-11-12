using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIndoor : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite closed, open;
    [SerializeField] MonoBehaviour shadowCaster2D;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            spriteRenderer.sprite = open;
            shadowCaster2D.enabled = false;
            Destroy(this);
        }
    }
}
