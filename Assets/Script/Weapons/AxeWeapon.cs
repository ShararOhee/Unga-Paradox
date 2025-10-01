using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // for Mouse.current

public class AxeWeapon : Weapon
{
    private float lastAttackTime = -999f;

    public override void Attack()
    {
        Debug.Log("Attack() called on " + weaponName);

        // Enforce cooldown
        if (Time.time - lastAttackTime < cooldown)
        {
            Debug.Log("Attack skipped: still on cooldown.");
            return;
        }

        // Trigger weapon animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log("Animator trigger sent for Attack()");
        }
        else
        {
            Debug.LogError("Animator is NULL on " + weaponName);
        }

        // Spawn slash effect prefab (if assigned)
        if (attackEffectPrefab != null)
        {
            // Get mouse world position
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;

            // Direction from weapon/player to mouse
            Vector2 direction = (mouseWorldPos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Spawn slash prefab
            Vector3 spawnPos = transform.position + (Vector3)direction * attackDistance;
            GameObject attack = Instantiate(attackEffectPrefab, spawnPos, Quaternion.Euler(0, 0, angle));
            attack.transform.SetParent(transform.root);

            // Play slash anim if Animator exists
            Animator slashAnim = attack.GetComponent<Animator>();
            if (slashAnim != null) slashAnim.SetTrigger("isAttack");

            Destroy(attack, attackLifetime);

            Debug.Log("Spawned slash effect for " + weaponName);
        }

        lastAttackTime = Time.time;

        // Hide axe after attack
        StartCoroutine(HideAfterAnimation(0.45f)); // tweak this timing as needed
    }

    private IEnumerator HideAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

        PlayerWeaponHolder holder = GetComponentInParent<PlayerWeaponHolder>();
        if (holder != null)
        {
            holder.HideWeapon();
        }
    }
}
