using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float waveHeight = 0.2f;
    [SerializeField] private float waveFrequency = 1f;
    [SerializeField] private float waveSpeed = 1f;
    
    [Header("Ship Motion")]
    [SerializeField] private float rollStrength = 2f;    // Side-to-side rotation
    [SerializeField] private float pitchStrength = 1f;   // Front-to-back rotation
    [SerializeField] private float motionSmoothing = 0.5f;
    
    [Header("Position Offset")]
    [SerializeField] private float verticalOffset = 0f;  // Base height above water
    
    // Internal variables
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float timeOffset;
    
    private void Start()
    {
        // Generate a random time offset for each ship to prevent synchronized motion
        timeOffset = Random.Range(0f, 100f);
    }
    
    private void Update()
    {
        float time = Time.time + timeOffset;
        
        // Calculate wave height at current position
        float xPos = transform.position.x;
        float zPos = transform.position.z;
        
        // Create more natural wave motion by combining multiple sine waves
        float waveOffset = 
            Mathf.Sin(time * waveFrequency + xPos * waveSpeed) * waveHeight * 0.6f +
            Mathf.Sin(time * waveFrequency * 0.8f + zPos * waveSpeed * 1.2f) * waveHeight * 0.4f;
        
        // Calculate target position with wave offset
        targetPosition = transform.position;
        targetPosition.y = waveOffset + verticalOffset;
        
        // Calculate rolling motion based on position and time
        float roll = 
            Mathf.Sin(time * waveFrequency * 0.7f + xPos * 0.2f) * rollStrength +
            Mathf.Sin(time * waveFrequency * 0.5f + zPos * 0.3f) * rollStrength * 0.5f;
            
        // Calculate pitching motion
        float pitch = 
            Mathf.Sin(time * waveFrequency * 0.6f + zPos * 0.2f) * pitchStrength +
            Mathf.Sin(time * waveFrequency * 0.4f + xPos * 0.3f) * pitchStrength * 0.5f;
            
        // Combine rotations
        targetRotation = Quaternion.Euler(
            pitch,
            transform.rotation.eulerAngles.y, // Preserve current yaw (steering)
            roll
        );
        
        // Smoothly interpolate to target position and rotation
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / motionSmoothing);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime / motionSmoothing);
    }
}