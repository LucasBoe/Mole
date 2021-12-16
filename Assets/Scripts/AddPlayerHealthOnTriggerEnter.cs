using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayerHealthOnTriggerEnter : MonoBehaviour
{
    [SerializeField] GameObject effect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            PlayerHealth.Instance.Heal(100);
            GameObject instance = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(instance, 2);
            Destroy(gameObject);
        }
    }
}
