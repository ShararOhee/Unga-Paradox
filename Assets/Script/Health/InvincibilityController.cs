using System.Collections;
using UnityEngine;

public class InvincibilityController : MonoBehaviour
{
    private Health healthController;
    private void Awake()
    {
        healthController = GetComponent<Health>();
    }

    public void StartInvincibility(float invincibilityDuration)
    {
        StartCoroutine(InvincibilityCoroutine(invincibilityDuration));
    }

    private IEnumerator InvincibilityCoroutine(float invincibilityDuration)
    {   
        healthController.isInvincible =  true;
        yield return new WaitForSeconds(invincibilityDuration);
        healthController.isInvincible = false;
    }
}
