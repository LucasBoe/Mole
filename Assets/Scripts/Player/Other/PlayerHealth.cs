using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    void Heal(int amount);
    void DoDamage(int amount);
}

public class PlayerHealth : PlayerSingletonBehaviour<PlayerHealth>, IHealth
{
    [SerializeField] int max;
    int current;
    float relative => current / (float)max;

    public System.Action<float> OnHealthChange;

    private void Start()
    {
        current = max;
    }

    public void Heal(int amount)
    {
        current = Mathf.Min(current += amount, max);
        OnHealthChange?.Invoke(relative);
    }

    public void DoDamage(int amount)
    {
#if UNITY_EDITOR
        if (DeveloperTools.HasGodMode)
            return;
#endif

        current -= amount;

        OnHealthChange?.Invoke(relative);

        if (current < 0)
        {
            PlayerSpawnHandler.Respawn();
        }
    }
}
