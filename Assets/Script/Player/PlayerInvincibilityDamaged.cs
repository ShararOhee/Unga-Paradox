using UnityEngine;

public class PlayerInvincibilityDamaged : MonoBehaviour
{
    [SerializeField]
    private float invincibilityDuration;
    
    private InvincibilityController invincibilityController;
    
    public void Awake()
    {
        invincibilityController = GetComponent<InvincibilityController>();
    }

    public void StartInvincibility()
    {
        invincibilityController.StartInvincibility(invincibilityDuration);
    }
}
