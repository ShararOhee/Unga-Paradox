using UnityEngine;

public class LightRotater : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 30.0f;
    public bool clockwise = true;
    
    void Update()
    {
        float direction = clockwise ? 1f : -1f;
        float rotationAmount = rotationSpeed * Time.deltaTime * direction;
        
        transform.Rotate(0, 0, rotationAmount);
    }
    
    // Quick methods to change behavior
    public void SetSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }
    
    public void SetDirection(bool isClockwise)
    {
        clockwise = isClockwise;
    }
}