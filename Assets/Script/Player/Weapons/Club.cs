using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MonoBehaviour, WeaponInterface
{
    public void Attack()
    {
        Debug.Log("Club Att");
        ActiveWeapon.Instance.ToggleIsAttacking(false);
    }
}
