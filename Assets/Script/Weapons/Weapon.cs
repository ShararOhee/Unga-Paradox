using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
    [Header("General Weapon Settings")]
    public string weaponName;
    public Sprite icon;               // For UI (hotbar/inventory)
    public float damage = 1f;
    public float cooldown = 0.5f;

    [Header("Animator Reference")]
    public Animator animator;         // Weapon-specific Animator

    [Header("Attack Effect Settings")]
    public GameObject attackEffectPrefab;   // Slash prefab to spawn
    public float attackDistance = 1.0f;     // Distance from player to spawn effect
    public float attackLifetime = 0.3f;     // How long slash stays alive
    public AudioClip attackSound;           // Optional sound

    protected AudioSource audioSource;
    protected float lastAttackTime;

    protected virtual void Awake()
    {
        // Cache AudioSource (add one if missing)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Called by PlayerWeaponHolder when player attacks.
    /// Must be implemented by subclasses (SwordWeapon, AxeWeapon, etc.)
    /// </summary>
    public abstract void Attack();

    /// <summary>
    /// Utility method for spawning a slash effect aimed at the mouse.
    /// Can be reused by all weapons that want slash animations.
    /// </summary>
    protected void SpawnSlashEffect(Vector3 origin)
    {
        if (attackEffectPrefab == null) return;

        // Get mouse world position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            UnityEngine.InputSystem.Mouse.current.position.ReadValue()
        );
        mouseWorldPos.z = 0f;

        // Direction from weapon/player to mouse
        Vector2 direction = (mouseWorldPos - origin).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Spawn slash prefab
        Vector3 spawnPos = origin + (Vector3)direction * attackDistance;
        GameObject attack = Instantiate(attackEffectPrefab, spawnPos, Quaternion.Euler(0, 0, angle));
        attack.transform.SetParent(transform.root); // attach to player

        // Play slash anim if Animator exists
        Animator slashAnim = attack.GetComponent<Animator>();
        if (slashAnim != null) slashAnim.SetTrigger("isAttack");

        Destroy(attack, attackLifetime);

        // Play sound
        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }
}
