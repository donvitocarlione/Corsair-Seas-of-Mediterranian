using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    public float waterDensity = 1.5f;
    public float waterLevel = 0f;
    public float buoyancyForce = 25f;  // Further reduced from 50
    public float waterDrag = 1.5f;     // Increased for more stability
    public float waterAngularDrag = 1.5f; // Increased for more stability

    [Header("Advanced Settings")]
    public float sideResistance = 3f;
    public float turningResistance = 2f;
    public bool useWaves = true;
    public float waveHeight = 0.1f;     // Further reduced from 0.2
    public float waveFrequency = 0.5f;  // Reduced for gentler waves

    [Header("Stabilization")]
    public float rollStability = 0.8f;   // Increased from 0.3
    public float pitchStability = 0.6f;  // Increased from 0.2

    [Header("Debug Info")]
    public float boatSubmergencePercentage = 10f;
    public bool isInWater = true;
    public Vector3 waterMovement = Vector3.zero;

    private Rigidbody rb;
    private float originalDrag;
    private float originalAngularDrag;
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Buoyancy requires a Rigidbody!");
            enabled = false;
            return;
        }

        // Store original values
        originalDrag = rb.drag;
        originalAngularDrag = rb.angularDrag;
        initialPosition = transform.position;

        // Set initial rigidbody properties
        rb.drag = waterDrag;
        rb.angularDrag = waterAngularDrag;
        
        // Ensure the ship starts stable
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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

        // Calculate buoyancy force with smoother transition
        float waterHeightWithWaves = waterLevel + waveOffset;
        float submergenceDepth = waterHeightWithWaves - transform.position.y;
        float submergencePercentage = Mathf.Clamp01(submergenceDepth / boatSubmergencePercentage);
        
        // Apply smoother buoyancy force
        float smoothedForce = Mathf.Lerp(0, buoyancyForce, submergencePercentage);
        Vector3 buoyancyVector = Vector3.up * smoothedForce;
        rb.AddForce(buoyancyVector, ForceMode.Force);

        // Apply stronger initial stabilization
        ApplyStabilization();

        // Strong damping for vertical movement
        Vector3 verticalVelocity = Vector3.up * Vector3.Dot(rb.velocity, Vector3.up);
        rb.AddForce(-verticalVelocity * waterDrag, ForceMode.Acceleration);

        // Store current water movement for debugging
        waterMovement = rb.velocity;
    }

    private void ApplyStabilization()
    {
        // Enhanced roll stabilization
        Vector3 rotation = transform.rotation.eulerAngles;
        if (rotation.z > 180) rotation.z -= 360;
        if (rotation.z != 0)
        {
            float stabilizationTorque = -rotation.z * rollStability;
            rb.AddTorque(0, 0, stabilizationTorque, ForceMode.Force);
            
            // Additional angular velocity damping
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * rollStability);
        }

        // Enhanced pitch stabilization
        if (rotation.x > 180) rotation.x -= 360;
        if (rotation.x != 0)
        {
            float stabilizationTorque = -rotation.x * pitchStability;
            rb.AddTorque(stabilizationTorque, 0, 0, ForceMode.Force);
        }

        // Maintain upright orientation
        Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rollStability);
    }

    void OnValidate()
    {
        // Ensure reasonable value ranges
        waterDrag = Mathf.Max(0, waterDrag);
        waterAngularDrag = Mathf.Max(0, waterAngularDrag);
        buoyancyForce = Mathf.Max(0, buoyancyForce);
        waveHeight = Mathf.Max(0, waveHeight);
        waveFrequency = Mathf.Max(0, waveFrequency);
    }
}
