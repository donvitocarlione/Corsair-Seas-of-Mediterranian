using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    public float waterDensity = 1.5f;
    public float waterLevel = 0f;
    public float buoyancyForce = 150f;  // Increased for better floating
    public float waterDrag = 3f;
    
    [Header("Wave Settings")]
    public bool useWaves = true;
    public float waveHeight = 0.2f;
    public float waveFrequency = 1f;

    [Header("Debug Info")]
    public float submergenceDepth;
    public bool isInWater = true;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Buoyancy requires a Rigidbody!");
            enabled = false;
            return;
        }
    }

    void FixedUpdate()
    {
        if (!isInWater) return;

        // Calculate wave offset
        float waveOffset = 0f;
        if (useWaves)
        {
            waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveHeight;
        }

        // Calculate and apply buoyancy
        float waterHeightWithWaves = waterLevel + waveOffset;
        submergenceDepth = waterHeightWithWaves - transform.position.y;
        
        if (submergenceDepth > 0)
        {
            // Basic water resistance
            rb.AddForce(-rb.velocity * waterDrag, ForceMode.Acceleration);
            
            // Main buoyancy force
            float buoyancyMultiplier = Mathf.Clamp01(submergenceDepth);
            Vector3 buoyancyForceVector = Vector3.up * buoyancyForce * buoyancyMultiplier;
            rb.AddForceAtPosition(buoyancyForceVector, transform.position, ForceMode.Force);
        }
    }
}
