using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthVisualizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer healthbarRenderer;
    [SerializeField] private EnemyDamageModule damageModule;
    private Material materialInstance;

    private void Start()
    {
        healthbarRenderer = Instantiate(healthbarRenderer, transform);
        materialInstance = new Material(healthbarRenderer.material);
        healthbarRenderer.material = materialInstance;
    }

    private void OnEnable()
    {
        damageModule.HealthChanged += UpdateHealthbar;
    }

    private void OnDisable()
    {
        damageModule.HealthChanged += UpdateHealthbar;
    }

    private void UpdateHealthbar(float healthRatio)
    {
        materialInstance.SetFloat("health", healthRatio);
    }
}
