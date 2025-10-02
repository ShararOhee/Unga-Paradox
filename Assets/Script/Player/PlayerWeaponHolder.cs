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

        // Rotate/align weapon toward mouse each frame
        if (currentWeapon != null)
        {
            UpdateWeaponOrientation();
        }

        // Left click to attack
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

    /// <summary>
    /// Rotates the weapon to face the mouse and offsets it outward a bit.
    /// </summary>
    private void UpdateWeaponOrientation()
    {
        if (currentWeapon == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
        mouseWorldPos.z = 0f;

        // Direction from player to mouse
        Vector3 origin = weaponParent.position;
        Vector2 dir = (mouseWorldPos - origin).normalized;

        // Flip player + weapon horizontally based on mouse side
        bool facingLeft = dir.x < 0;

        // Flip the weapon sprite
        SpriteRenderer sr = currentWeapon.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = facingLeft;
        }

        // Offset weapon outward (left or right of player)
        float offsetDist = 0.5f; // tweak per weapon
        currentWeapon.transform.localPosition = new Vector3(
            facingLeft ? -offsetDist : offsetDist,
            0f,
            0f
        );

        // Optional: small tilt up/down depending on mouse y
        float tilt = Mathf.Clamp(dir.y * 30f, -30f, 30f); // -30° to +30°
        currentWeapon.transform.rotation = Quaternion.Euler(0, 0, tilt);
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
