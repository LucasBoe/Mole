using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPlayerHealthOnTriggerEnter : MonoBehaviour
{
    [SerializeField] ParticleSystem gainHealthEffectPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            PlayerHealth.Instance.Heal(100);
            EffectHandler.Spawn(new CustomEffect(gainHealthEffectPrefab), transform.position);
            Destroy(gameObject);
        }
    }
}
