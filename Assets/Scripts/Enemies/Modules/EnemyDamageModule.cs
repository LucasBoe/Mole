using System;
using UnityEngine;

public class EnemyDamageModule : EnemyModule<EnemyDamageModule>, IHealth
{
    [SerializeField] private int maxHealth;
    [SerializeField, ReadOnly] private int currentHealth;
    [SerializeField] EnemyFallDamageModule falldamage;

    public System.Action OutOfHealth;
    public System.Action<float> HealthChanged;

    public bool Dead => currentHealth <= 0f;

    protected override void Awake()
    {
        if (falldamage != null)
            falldamage = Instantiate(falldamage, transform);

        base.Awake();
    }

    internal void Kill()
    {
        DoDamage(int.MaxValue);
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void DoDamage(int amount)
    {

        Debug.LogWarning($"DoDamage {amount}");

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDamager damager = collision.GetComponentInChildren<EnemyDamager>();
        if (damager != null)
        {

            DoDamage(damager.FetchDamageAmout());
        }
    }
}
