using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 2.0f;
    public float detectionRange = 10.0f;
    
    private bool targetDetected = false;
    
    void Update()
    {
        if (target == null) return;
        
        float distance = Vector3.Distance(transform.position, target.position);
        
        // Once target is in range, keep rotating forever
        if (distance <= detectionRange)
        {
            targetDetected = true;
        }
        
        if (targetDetected)
        {
            Rotate();
        }
    }
    
    void Rotate()
    {
        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // For 2D sprites, you might need to adjust this offset (-90) depending on your sprite's default orientation
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + 90f);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = targetDetected ? Color.red : Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}