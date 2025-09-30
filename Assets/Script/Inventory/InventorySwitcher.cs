using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySwitcher : MonoBehaviour
{
    private GameObject[] inventorySlots;
    private int activeSlot = -1; // -1 means no active slot

    public PlayerWeaponHolder weaponHolder; // drag your Player here in Inspector

    void Start()
    {
        inventorySlots = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            inventorySlots[i] = transform.GetChild(i).gameObject;

        UpdateActiveSlot(-1); // nothing active at start
        if (weaponHolder != null)
        {
            weaponHolder.allowAttack = false;
            weaponHolder.EquipWeapon(null);
        }
    }

    void Update()
    {
        var k = Keyboard.current;
        if (k.digit1Key.wasPressedThisFrame) SwitchSlot(0);
        if (k.digit2Key.wasPressedThisFrame) SwitchSlot(1);
        if (k.digit3Key.wasPressedThisFrame) SwitchSlot(2);
        if (k.digit4Key.wasPressedThisFrame) SwitchSlot(3);
        if (k.digit5Key.wasPressedThisFrame) SwitchSlot(4);
    }

    void SwitchSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length) return;

        // If clicking the same slot, toggle OFF
        if (activeSlot == slotIndex)
        {
            activeSlot = -1;
            UpdateActiveSlot(-1);

            if (weaponHolder != null)
            {
                weaponHolder.allowAttack = false;    // "Active" is off → no attacking
                weaponHolder.EquipWeapon(null);      // clear pending and destroy current if any
            }
            return;
        }

        // Activate new slot
        activeSlot = slotIndex;
        UpdateActiveSlot(activeSlot);

        // Look up the prefab reference on this slot's "Item"
        Transform itemChild = inventorySlots[slotIndex].transform.Find("Item");
        Weapon prefab = null;
        if (itemChild != null)
        {
            var refComp = itemChild.GetComponent<ItemWeaponRef>();
            if (refComp != null) prefab = refComp.weaponPrefab;
        }

        if (weaponHolder != null)
        {
            // With Active ON: allow attacking only if we actually have a weapon prefab
            weaponHolder.allowAttack = (prefab != null);
            weaponHolder.EquipWeapon(prefab); // store pending (no spawn yet)
        }
    }

    // Turns each slot's "Active" child on only for the selected index; off for others.
    void UpdateActiveSlot(int slotIndex)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Transform activeChild = inventorySlots[i].transform.Find("Active");
            if (activeChild != null)
                activeChild.gameObject.SetActive(i == slotIndex && slotIndex != -1);
        }
    }
}
