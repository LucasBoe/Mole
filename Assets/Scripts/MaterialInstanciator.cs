using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstanciator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Material mat = new Material(spriteRenderer.material);
        spriteRenderer.material = mat;
    }
}
