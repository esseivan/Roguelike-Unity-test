using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    /// <summary>
    /// The maximum health the character can have
    /// </summary>
    public float maxHealth = 100;

    /// <summary>
    /// The current health of the character
    /// </summary>
    public float health = 0;

    /// <summary>
    /// The OnDied event, called when health reachs 0 through TakeDamage method
    /// </summary>
    public event EventHandler OnDied = null;

    private void Start()
    {
        // Set currentHealth to maxHealth at start
        health = maxHealth;
    }

    /// <summary>
    /// Take damage
    /// </summary>
    /// <param name="amount">Damage to take, greater than 0</param>
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

    /// <summary>
    /// Heal the character
    /// </summary>
    /// <param name="amount">Health to give, greater than 0</param>
    public void GiveHealth(float amount)
    {
        if (amount <= 0)
            return;

        health += amount;

        // If overflow, set to max
        if (health > maxHealth)
            health = maxHealth;
    }

    /// <summary>
    /// Set the current health
    /// </summary>
    /// <param name="amount">Health, greater or equal than 0, lesser than maxHealth. Doesn't trigger the OnDied event</param>
    public void SetHealth(float amount)
    {
        if (amount < 0)
            return;

        health = amount;

        if (health > maxHealth)
            maxHealth = health;
    }
}
