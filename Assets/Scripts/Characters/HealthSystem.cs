using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100;

    public float health = 0;

    public event EventHandler OnDied = null;

    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0)
            return;

        health -= amount;

        if(health <= 0)
        {
            health = 0;
            OnDied?.Invoke(this.gameObject, EventArgs.Empty);
        }
    }

    public void GiveHealth(float amount)
    {
        if (amount <= 0)
            return;

        health += amount;

        if (health > maxHealth)
            health = maxHealth;
    }

    public void SetHealth(float amount)
    {
        if (amount < 0)
            return;

        health = amount;

        if (health > maxHealth)
            maxHealth = health;
    }
}
