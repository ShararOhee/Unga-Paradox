using UnityEngine;

public class HealthAudio : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] hurtClips;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip healClip;
    
    [Header("Tuning")]
    [Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Random +/- pitch to avoid repetitiveness.")]
    [Range(0f, 0.5f)] public float pitchVariance = 0.1f;
    [Tooltip("Small gap to prevent multiple hurt sounds in the same frame burst.")]
    [SerializeField] private float minGapSeconds = 0.05f;

    private float _lastPlayTime = -999f;

    private void Reset()
    {
        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        // For 2D UI sounds keep Spatial Blend = 0, for world sounds set > 0
    }

    public void PlayHurt()
    {
        if (hurtClips == null || hurtClips.Length == 0) return;
        if (Time.time - _lastPlayTime < minGapSeconds) return;

        var clip = hurtClips[Random.Range(0, hurtClips.Length)];
        source.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        source.PlayOneShot(clip, volume);
        _lastPlayTime = Time.time;
    }

    public void PlayHeal()
    {
        if (healClip == null) return;
        
        if (healClip == null)
        {
            PlayHeal();
            return;
        }
        
        if (Time.time - _lastPlayTime < minGapSeconds) return;
        
        source.pitch = 1f;
        source.PlayOneShot(healClip, volume);
    }

    public void PlayDeath()
    {
        if (deathClip == null)
        {
            PlayHurt();
            return;
        }
        source.pitch = 1f;
        source.PlayOneShot(deathClip, volume);
    }
}