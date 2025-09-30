using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public Sprite icon;
    public float damage;
    public float cooldown;
    public Animator animator;

    protected float lastAttackTime;

    public abstract void Attack();
}
