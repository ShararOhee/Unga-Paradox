using UnityEngine;
using UnityEngine.InputSystem; // for new Input System

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    public Animator weaponAnimator; // drag Weapon’s Animator here in Inspector

    [Header("Settings")]
    public float attackCooldown = 0.5f; // time between attacks

    private float lastAttackTime;

    void Update()
    {
        // check for left mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        // check if enough time has passed since last attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            weaponAnimator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }
}
