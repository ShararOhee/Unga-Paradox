using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeaponHolder : MonoBehaviour
{
    // Chosen prefab from the hotbar; we only spawn it on first click
    private Weapon pendingWeaponPrefab;

    [Header("Attach point")]
    public Transform weaponParent;

    [Header("Runtime")]
    public Weapon currentWeapon;

    // Set by InventorySwitcher based on the "Active" highlight
    [HideInInspector] public bool allowAttack = false;

    void Update()
    {
        if (!allowAttack) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Spawn only when first attack is triggered
            if (currentWeapon == null && pendingWeaponPrefab != null)
            {
                GameObject newWeaponObj = Instantiate(pendingWeaponPrefab.gameObject, weaponParent);
                currentWeapon = newWeaponObj.GetComponent<Weapon>();
                currentWeapon.transform.localPosition = Vector3.zero;
                currentWeapon.transform.localRotation = Quaternion.identity;
                currentWeapon.transform.localScale = Vector3.one;
            }

            if (currentWeapon != null)
                currentWeapon.Attack();
        }
    }

    /// Called by InventorySwitcher when a slot is selected (passes prefab) or deselected (passes null).
    public void EquipWeapon(Weapon weaponPrefabOrNull)
    {
        if (weaponPrefabOrNull == null)
        {
            // Deselect: clear pending and any current instance
            pendingWeaponPrefab = null;
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
                currentWeapon = null;
            }
            return;
        }

        // Switch selection: clear current instance, store new pending
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
            currentWeapon = null;
        }
        pendingWeaponPrefab = weaponPrefabOrNull;
    }

    // Called by weapon after its attack finishes (animation/coroutine)
    public void HideWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
            currentWeapon = null;
        }
    }
}
