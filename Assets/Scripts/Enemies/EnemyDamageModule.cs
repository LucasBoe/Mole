using UnityEngine;

public class EnemyDamageModule : EnemyModule<EnemyDamageModule>, IHealth
{
    [SerializeField] private int maxHealth;
    [SerializeField, ReadOnly] private int currentHealth;

    public System.Action OutOfHealth;
    public System.Action<float> HealthChanged;

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
