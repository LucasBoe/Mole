using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnHitBehaviour : MonoBehaviour
{
    [SerializeField] float velocityNeeded = 10f;
    [SerializeField] GameObject prefab;
    [SerializeField] float noise;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<Rigidbody2D>().velocity.magnitude > velocityNeeded)
            DestroyAndReplace();
    }

    private void DestroyAndReplace()
    {
        if (prefab != null)
            Instantiate(prefab, transform.position, Quaternion.identity);

        if (noise > 0f)
            NoiseHandler.Instance.MakeNoise(transform.position, noise);

        Destroy(gameObject);
    }
}
