using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 100;
    public AudioClip deathSound;
    public AudioClip hurtSound1;
    public AudioClip hurtSound2;
    public AudioClip hurtSound3;
    public AudioClip[] hurtSounds;
    public int currentHealth;

    private AudioSource audioSource;

    private void Start()
    {
        currentHealth = startingHealth;
        audioSource = GetComponent<AudioSource>();
        hurtSounds = new AudioClip[] { hurtSound1, hurtSound2, hurtSound3 };
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0)
            audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
        Debug.Log(currentHealth);
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            audioSource.PlayOneShot(deathSound);
            Destroy(gameObject, deathSound.length);
        }
    }
}
