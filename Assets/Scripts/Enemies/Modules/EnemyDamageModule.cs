using System;
using UnityEngine;
using NaughtyAttributes;

public class EnemyDamageModule : EnemyModule<EnemyDamageModule>, IHealth
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] EnemyFallDamageModule falldamage;
    [SerializeField] EnemyDamageTrigger damageTrigger;
    public EnemyDamageTrigger DamageTrigger => damageTrigger;

    public System.Action OutOfHealth;
    public System.Action<float> HealthChanged;

    [ShowNativeProperty]
    public bool Dead => currentHealth <= 0f;

    protected override void Awake()
    {
        if (falldamage != null)
            falldamage = Instantiate(falldamage, transform);

        damageTrigger = Instantiate(damageTrigger, transform);
        damageTrigger.TriggerEntered += OnTriggerEntered;

        currentHealth = maxHealth;

        base.Awake();
    }

    internal void Kill()
    {
        DoDamage(int.MaxValue);
    }

    public void DoDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            OutOfHealth?.Invoke();

        HealthChanged?.Invoke((float)currentHealth / maxHealth);
    }
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        HealthChanged?.Invoke((float)currentHealth / maxHealth);
    }

    private void OnTriggerEntered(EnemyDamager damager)
    {
        DoDamage(damager.FetchDamageAmout());
    }
}
