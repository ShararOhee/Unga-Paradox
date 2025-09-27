using UnityEngine;
using System.Collections;

public class DelayedMusicPlayer : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip musicClip;
    public float delayInSeconds = 2.0f;
    
    [Header("Audio Source (Optional)")]
    public AudioSource audioSource; // Assign if you have a specific AudioSource
    
    void Start()
    {
        // If no AudioSource is assigned, get or create one
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Start the delay coroutine
        StartCoroutine(PlayMusicAfterDelay());
    }
    
    IEnumerator PlayMusicAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayInSeconds);
        
        // Play the music
        if (musicClip != null && audioSource != null)
        {
            audioSource.clip = musicClip;
            audioSource.Play();
        }
    }
}