using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float waveHeight = 0.2f;
    [SerializeField] private float waveFrequency = 1f;
    [SerializeField] private float waveSpeed = 1f;
    [SerializeField] private float waterLevel = 0f;
    
    [Header("Ship Motion")]
    [SerializeField] private float rollStrength = 1f;    // Reduced for more stability
    [SerializeField] private float pitchStrength = 0.5f; // Reduced for more stability
    [SerializeField] private float motionSmoothing = 0.5f;
    
    [Header("Position Offset")]
    [SerializeField] private float verticalOffset = 4.5f;  // Increased to lift ships higher
    
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float timeOffset;
    
    public float WaterLevel => waterLevel;
    
    private void Start()
    {
        timeOffset = Random.Range(0f, 100f);
        
        // Set initial position above water
        Vector3 pos = transform.position;
        pos.y = waterLevel + verticalOffset;
        transform.position = pos;

        // Log initial position for debugging
        Debug.Log($"[Buoyancy] Initial position set to {pos} (water level: {waterLevel}, offset: {verticalOffset})");
    }
    
    private void LateUpdate()
    {
        float time = Time.time + timeOffset;
        float xPos = transform.position.x;
        float zPos = transform.position.z;
        
        // Calculate wave offset
        float waveOffset = 
            Mathf.Sin(time * waveFrequency + xPos * waveSpeed) * waveHeight * 0.6f +
            Mathf.Sin(time * waveFrequency * 0.8f + zPos * waveSpeed * 1.2f) * waveHeight * 0.4f;
        
        // Calculate target position while maintaining current x and z
        targetPosition = transform.position;
        float targetY = waterLevel + verticalOffset + waveOffset;
        targetPosition.y = targetY;
        
        // Calculate ship rotation
        float roll = 
            Mathf.Sin(time * waveFrequency * 0.7f + xPos * 0.2f) * rollStrength +
            Mathf.Sin(time * waveFrequency * 0.5f + zPos * 0.3f) * rollStrength * 0.5f;
            
        float pitch = 
            Mathf.Sin(time * waveFrequency * 0.6f + zPos * 0.2f) * pitchStrength +
            Mathf.Sin(time * waveFrequency * 0.4f + xPos * 0.3f) * pitchStrength * 0.5f;
            
        targetRotation = Quaternion.Euler(
            pitch,
            transform.rotation.eulerAngles.y,
            roll
        );
        
        // Apply position and rotation changes
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / motionSmoothing);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime / motionSmoothing);

        // Log position if it's too low (for debugging)
        if (transform.position.y < waterLevel)
        {
            Debug.LogWarning($"[Buoyancy] Ship is below water level! Position: {transform.position.y}, Water Level: {waterLevel}");
        }
    }

    private void OnValidate()
    {
        // Ensure reasonable values in the inspector
        if (verticalOffset < 0)
        {
            verticalOffset = 4.5f;
            Debug.LogWarning("[Buoyancy] Vertical offset should be positive. Reset to default.");
        }

        if (waveHeight > 1f)
        {
            waveHeight = 0.2f;
            Debug.LogWarning("[Buoyancy] Wave height too large. Reset to default.");
        }
    }
}