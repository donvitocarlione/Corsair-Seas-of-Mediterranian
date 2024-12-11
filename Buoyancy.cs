using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    public float waterDensity = 1.5f;
    public float waterLevel = 0f;
    public float buoyancyForce = 50f;  // Reduced from 100
    public float waterDrag = 1f;        // Reduced from 2
    public float waterAngularDrag = 1f; // Reduced from 2

    [Header("Advanced Settings")]
    public float sideResistance = 3f;
    public float turningResistance = 2f;
    public bool useWaves = true;
    public float waveHeight = 0.2f;     // Reduced from 0.5
    public float waveFrequency = 1f;

    [Header("Stabilization")]
    public float rollStability = 0.3f;
    public float pitchStability = 0.2f;

    [Header("Debug Info")]
    public float boatSubmergencePercentage = 10f;
    public bool isInWater = true;
    public Vector3 waterMovement = Vector3.zero;

    private Rigidbody rb;
    private float originalDrag;
    private float originalAngularDrag;

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

        // Set initial rigidbody properties
        rb.drag = waterDrag;
        rb.angularDrag = waterAngularDrag;
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

        // Calculate buoyancy force
        float waterHeightWithWaves = waterLevel + waveOffset;
        float submergenceDepth = waterHeightWithWaves - transform.position.y;
        float submergencePercentage = Mathf.Clamp01(submergenceDepth / boatSubmergencePercentage);

        // Apply buoyancy force
        Vector3 buoyancyVector = Vector3.up * buoyancyForce * submergencePercentage;
        rb.AddForce(buoyancyVector, ForceMode.Force);

        // Apply stabilization
        ApplyStabilization();

        // Store current water movement for debugging
        waterMovement = rb.velocity;
    }

    private void ApplyStabilization()
    {
        // Roll stabilization
        Vector3 rotation = transform.rotation.eulerAngles;
        if (rotation.z > 180) rotation.z -= 360;
        if (rotation.z != 0)
        {
            float stabilizationTorque = -rotation.z * rollStability;
            rb.AddTorque(0, 0, stabilizationTorque, ForceMode.Force);
        }

        // Pitch stabilization
        if (rotation.x > 180) rotation.x -= 360;
        if (rotation.x != 0)
        {
            float stabilizationTorque = -rotation.x * pitchStability;
            rb.AddTorque(stabilizationTorque, 0, 0, ForceMode.Force);
        }
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
