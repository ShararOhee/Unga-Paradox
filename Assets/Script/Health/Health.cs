using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public bool isInvincible;

    public float RemainingHealthPercentage
    {
        get { return currentHealth / maxHealth; }
    }

    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;
    
    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            return;
        }
        
        if(isInvincible) 
        {
            return;
        }
        
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (currentHealth == 0)
        {
            OnDeath.Invoke();
        }
        else
        {
            OnDamaged.Invoke();
        }
    }

    public void AddHealth(float amount)
    {
        if (currentHealth == maxHealth)
        {
            return;
        }
        
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    
}