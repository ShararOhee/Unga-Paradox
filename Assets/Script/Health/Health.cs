using System;                         // for Action<>
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Health state for a game object. Supports lives/respawn and invincibility.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Team Settings")]
    [Tooltip("The team associated with this damage")]
    public int teamId = 0;

    [Header("Health Settings")]
    [Tooltip("The default health value")]
    public int defaultHealth = 1;

    [Tooltip("The maximum health value")]
    public int maximumHealth = 1;

    [Tooltip("The current in game health value")]
    public int currentHealth = 1;

    [Tooltip("Invulnerability duration, in seconds, after taking damage")]
    public float invincibilityTime = 3f;

    [Header("Lives settings")]
    [Tooltip("Whether or not to use lives")]
    public bool useLives = false;

    [Tooltip("Current number of lives this health has")]
    public int currentLives = 3;

    [Tooltip("The maximum number of lives this health has")]
    public int maximumLives = 5;

    [Tooltip("The amount of time to wait before respawning")]
    public float respawnWaitTime = 3f;

    [Header("Effects & Polish")]
    [Tooltip("The effect to create when this health dies")]
    public GameObject deathEffect;

    [Tooltip("The effect to create when this health is damaged (but does not die)")]
    public GameObject hitEffect;

    public event Action<int, int> OnHealthChanged;

    // Tutorial-style wrappers so HealthBar can call Hp/MaxHp
    public int MaxHp
    {
        get => maximumHealth;
        set
        {
            maximumHealth = Mathf.Max(1, value);
            currentHealth = Mathf.Clamp(currentHealth, 0, maximumHealth);
            OnHealthChanged?.Invoke(currentHealth, maximumHealth);
        }
    }

    public int Hp
    {
        get => currentHealth;
        set
        {
            int v = Mathf.Clamp(value, 0, maximumHealth);
            if (v != currentHealth)
            {
                currentHealth = v;
                OnHealthChanged?.Invoke(currentHealth, maximumHealth);
            }
        }
    }

    // runtime state
    private float respawnTime;
    private Vector3 respawnPosition;
    private float timeToBecomeDamagableAgain = 0f;
    public bool isInvincible = false;

    private void Start()
    {
        SetRespawnPoint(transform.position);

        // Normalize and ping initial state so UI can display correct values
        maximumHealth = Mathf.Max(1, maximumHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maximumHealth);
        OnHealthChanged?.Invoke(currentHealth, maximumHealth);
    }

    private void Update()
    {
        InvincibilityCheck();
        RespawnCheck();
    }

    private void InvincibilityCheck()
    {
        if (timeToBecomeDamagableAgain <= Time.time)
        {
            isInvincible = false;
        }
    }

    private void RespawnCheck()
    {
        if (respawnWaitTime != 0 && currentHealth <= 0 && currentLives > 0)
        {
            if (Time.time >= respawnTime)
            {
                Respawn();
            }
        }
    }

    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    private void Respawn()
    {
        transform.position = respawnPosition;
        currentHealth = defaultHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, maximumHealth);
        OnHealthChanged?.Invoke(currentHealth, maximumHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible || currentHealth <= 0) return;

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, transform.rotation, null);

        timeToBecomeDamagableAgain = Time.time + invincibilityTime;
        isInvincible = true;

        currentHealth = Mathf.Clamp(currentHealth - Mathf.Abs(damageAmount), 0, maximumHealth);
        OnHealthChanged?.Invoke(currentHealth, maximumHealth);

        CheckDeath();
    }

    public void ReceiveHealing(int healingAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + Mathf.Abs(healingAmount), 0, maximumHealth);
        OnHealthChanged?.Invoke(currentHealth, maximumHealth);
        CheckDeath();
    }

    private bool CheckDeath()
    {
        if (currentHealth > 0) return false;

        // Death effects
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, transform.rotation, null);

        if (useLives)
        {
            currentLives = Mathf.Max(0, currentLives - 1);
            if (currentLives > 0)
            {
                if (respawnWaitTime == 0f) Respawn();
                else respawnTime = Time.time + respawnWaitTime;
            }
            else
            {
                if (respawnWaitTime == 0f) Destroy(gameObject);
                else respawnTime = Time.time + respawnWaitTime;
            }
        }
        else
        {
            Destroy(gameObject);
        }
        return true;
    }
}
