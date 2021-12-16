using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : SingletonBehaviour<PlayerHealth>
{
    [SerializeField] int max;
    int current;
    float relative => current / (float)max;

    public System.Action<float> OnHealthChange;

    private void Start()
    {
        current = max;
    }

    internal void Heal(int amount)
    {
        current = Mathf.Min(current += amount, max);
        OnHealthChange?.Invoke(relative);
    }

    public void DoDamage(int amount)
    {
        current -= amount;
        OnHealthChange?.Invoke(relative);

        if (current < 0)
            Destroy(gameObject);
    }
}
