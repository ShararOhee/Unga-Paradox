using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class which destroys it's gameobject after a certain amount of time, triggered by a button click
/// </summary>
public class TimedObjectDestroyer : MonoBehaviour
{
    [Header("Settings:")]
    [Tooltip("The lifetime of this gameobject")]
    public float lifetime = 5.0f;

    [Header("Button Trigger:")]
    [Tooltip("The button that will trigger the countdown when clicked")]
    public Button triggerButton;

    [Tooltip("Whether or not to destroy child gameobjects when this gameobject is destroyed")]
    public bool destroyChildrenOnDeath = true;

    [Tooltip("Whether the countdown should start automatically or wait for button click")]
    public bool startOnAwake = false;

    // The amount of time this gameobject has already existed in play mode
    private float timeAlive = 0.0f;
    private bool countdownStarted = false;

    /// <summary>
    /// Description:
    /// Standard Unity function called when the script is first loaded
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    void Awake()
    {
        // Set up button listener if a button is assigned
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(StartCountdown);
        }

        // Start countdown immediately if configured to do so
        if (startOnAwake)
        {
            StartCountdown();
        }
    }

    /// <summary>
    /// Description:
    /// Starts the countdown timer
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void StartCountdown()
    {
        if (!countdownStarted)
        {
            countdownStarted = true;
            timeAlive = 0.0f;
            Debug.Log("Countdown started! Object will be destroyed in " + lifetime + " seconds.");
        }
    }

    /// <summary>
    /// Description:
    /// Stops the countdown timer
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void StopCountdown()
    {
        countdownStarted = false;
        Debug.Log("Countdown stopped!");
    }

    /// <summary>
    /// Description:
    /// Resets the countdown timer
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void ResetCountdown()
    {
        timeAlive = 0.0f;
        countdownStarted = false;
        Debug.Log("Countdown reset!");
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once every frame
    /// Input: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    void Update()
    {
        // Only count if the countdown has been started
        if (countdownStarted)
        {
            // Every frame, increment the amount of time that this gameobject has been alive,
            // or if it has exceeded it's maximum lifetime, destroy it
            if (timeAlive > lifetime)
            {
                Destroy(this.gameObject);
            }
            else
            {
                timeAlive += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Description:
    /// Sets the trigger button programmatically
    /// Input: 
    /// Button newButton - the button to use as trigger
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void SetTriggerButton(Button newButton)
    {
        // Remove listener from old button if it exists
        if (triggerButton != null)
        {
            triggerButton.onClick.RemoveListener(StartCountdown);
        }

        // Set new button and add listener
        triggerButton = newButton;
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(StartCountdown);
        }
    }

    /// <summary>
    /// Description:
    /// Sets the lifetime programmatically
    /// Input: 
    /// float newLifetime - the new lifetime value
    /// Returns: 
    /// void (no return)
    /// </summary>
    public void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
    }

    /// <summary>
    /// Description:
    /// Gets the remaining time before destruction
    /// Input: 
    /// none
    /// Returns: 
    /// float - remaining time in seconds
    /// </summary>
    public float GetRemainingTime()
    {
        return Mathf.Max(0f, lifetime - timeAlive);
    }

    /// <summary>
    /// Description:
    /// Gets the progress of the countdown (0 to 1)
    /// Input: 
    /// none
    /// Returns: 
    /// float - progress from 0 to 1
    /// </summary>
    public float GetProgress()
    {
        if (lifetime <= 0) return 1f;
        return Mathf.Clamp01(timeAlive / lifetime);
    }

    // Flag which tells whether the application is shutting down (helps avoid errors)
    public static bool quitting = false;

    /// <summary>
    /// Description:
    /// Ensures that the quitting flag gets set correctly to avoid work as the application quits
    /// Input: 
    /// none
    /// Return: 
    /// void (no return)
    /// </summary>
    private void OnApplicationQuit()
    {
        quitting = true;
        DestroyImmediate(this.gameObject);
    }

    /// <summary>
    /// Description:
    /// Behavior which triggers when this component is destroyed
    /// Input: 
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void OnDestroy()
    {
        // Clean up button listener
        if (triggerButton != null)
        {
            triggerButton.onClick.RemoveListener(StartCountdown);
        }

        if (destroyChildrenOnDeath && !quitting && Application.isPlaying)
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject childObject = transform.GetChild(i).gameObject;
                if (childObject != null)
                {
                    Destroy(childObject);
                }
            }
        }
        transform.DetachChildren();
    }
}