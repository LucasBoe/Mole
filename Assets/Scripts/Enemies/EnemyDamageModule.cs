using UnityEngine;

public class EnemyDamageModule : EnemyModule<EnemyDamageModule>, IHealth
{
    [SerializeField] private int maxHealth;
    private int currentHealth;

    public System.Action OnOutOfHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void DoDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            OnOutOfHealth?.Invoke();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDamager damager = collision.GetComponent<EnemyDamager>();
        if (damager != null)
        {
            DoDamage(damager.FetchDamageAmout());
        }
    }
}
