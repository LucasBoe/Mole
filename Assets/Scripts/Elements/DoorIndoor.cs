using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIndoor : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite closed, open;
    [SerializeField] GameObject viewBlocker;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.IsPlayer())
        {
            spriteRenderer.sprite = open;
            viewBlocker.SetActive(false);
            Destroy(this);
        }
    }
}
