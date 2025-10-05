﻿using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    private PlayerControls controls;    // input actions
    private float timer = 0.0f;         // timer for attack cooldown
    private bool playOnce = false;      // bool for attack cooldown

    [Header("Attack Settings")]
    [Tooltip("Attack effect")]
    public GameObject attackEffect;
    [Tooltip("Attack sound")]
    public AudioClip attackSound;
    //[Tooltip("Attack Collider")]
    //public GameObject attackCollider;
    [Tooltip("Attack distance")]
    public float attackDistance = 1.0f;
    [Tooltip("Attack damage")]
    public float attackDamage = 1.0f;
    [Tooltip("Attack cooldown")]
    public float attackCooldown = 1.0f;

    private AudioSource audioSource;

    private void Awake()
    {
        controls = new PlayerControls();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Attack.performed += onAttack;
    }

    private void OnDisable()
    {
        controls.Player.Attack.performed -= onAttack;
        controls.Player.Disable();
    }

    private void Update()
    {
        // Attack Cooldown
        if (timer > attackCooldown)
        {
            if (!playOnce)
            {
                Debug.Log("Attack Enable " + timer);
                controls.Player.Attack.Enable();
                playOnce = true;
            }
        }
        if (timer < attackCooldown)
        {
            if (playOnce)
            {
                Debug.Log("Attack Disable " + timer);
                controls.Player.Attack.Disable();
                playOnce = false;
            }
        }
        timer += Time.deltaTime;
    }

    private void onAttack(InputAction.CallbackContext context)
    {
        // Reset timer for cooldown
        timer = 0.0f;

        // Get mouse position in world
        Vector2 mouseScreenPos = controls.Player.MousePosition.ReadValue<Vector2>();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // Direction from player to mouse
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        // Angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Spawn position = player position + offset in mouse direction
        Vector3 spawnPos = transform.position + (Vector3)direction * attackDistance;

        // Instantiate attack effect
        GameObject attack = Instantiate(attackEffect, spawnPos, Quaternion.Euler(0, 0, angle));
        attack.transform.SetParent(transform);

        // Play attack sound
        if (audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }
}
