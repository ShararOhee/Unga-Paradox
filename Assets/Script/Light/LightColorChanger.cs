using UnityEngine;

public class LightColorChanger : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The target GameObject to detect
    
    [Header("Light Settings")]
    public float detectionRange = 10.0f; // Range at which color changes
    public Color normalColor = Color.white; // Default light color
    public Color detectedColor = Color.red; // Color when target is in range
    
    [Header("Color Transition")]
    public float colorChangeSpeed = 2.0f; // How fast color transitions
    
    private Component spotlight;
    private bool targetInRange = false;
    private System.Reflection.PropertyInfo colorProperty;
    
    void Start()
    {
        // Try to get the Light2D component using reflection
        spotlight = GetComponent("Light2D");
        
        if (spotlight != null)
        {
            // Get the color property using reflection
            colorProperty = spotlight.GetType().GetProperty("color");
            if (colorProperty != null)
            {
                colorProperty.SetValue(spotlight, normalColor);
            }
            else
            {
                Debug.LogWarning("Color property not found on Light2D component!");
                spotlight = null;
            }
        }
        else
        {
            Debug.LogWarning("Light2D component not found on this GameObject! Make sure you have the 2D Renderer package installed.");
        }
    }
    
    void Update()
    {
        if (target == null || spotlight == null)
            return;
            
        // Calculate distance to target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        // Check if target is in range
        bool wasInRange = targetInRange;
        targetInRange = distanceToTarget <= detectionRange;
        
        // Handle color transition
        HandleColorTransition();
        
        // Optional: Trigger events when state changes
        if (!wasInRange && targetInRange)
        {
            OnTargetEnterRange();
        }
        else if (wasInRange && !targetInRange)
        {
            OnTargetExitRange();
        }
    }
    
    void HandleColorTransition()
    {
        if (colorProperty == null) return;
        
        Color targetColor = targetInRange ? detectedColor : normalColor;
        Color currentColor = (Color)colorProperty.GetValue(spotlight);
        
        // Smoothly transition between colors
        Color newColor = Color.Lerp(currentColor, targetColor, colorChangeSpeed * Time.deltaTime);
        colorProperty.SetValue(spotlight, newColor);
    }
    
    // Event methods that you can extend
    void OnTargetEnterRange()
    {
        Debug.Log("Target entered spotlight range!");
    }
    
    void OnTargetExitRange()
    {
        Debug.Log("Target left spotlight range!");
    }
    
    // Visualize detection range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = targetInRange ? detectedColor : normalColor;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        if (targetInRange && target != null)
        {
            Gizmos.color = detectedColor;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}