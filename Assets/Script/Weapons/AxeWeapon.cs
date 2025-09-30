using UnityEngine;
using System.Collections;

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

        // Trigger attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log("Animator trigger sent for Attack()");

            // Instead of relying on state length, just hardcode a delay
            StartCoroutine(HideAfterAnimation(0.45f)); // tweak this value!
        }
        else
        {
            Debug.LogError("Animator is NULL on " + weaponName);
        }

        lastAttackTime = Time.time;
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
