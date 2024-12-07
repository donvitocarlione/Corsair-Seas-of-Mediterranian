using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float waterDensity = 1000f; // Density of water (kg/m^3)
    public float waterLevelY = 0f; // Y-coordinate of the water surface
    public float boatSubmergedPercentage = 0f; // Percentage submerged (0.0 - 1.0)
    public bool isInWater; // Made public as per user request
    private Rigidbody rb;
    private Collider boatCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boatCollider = GetComponent<Collider>();

        if (rb == null)
        {
            Debug.LogError("Ship needs a Rigidbody!");
        }

        if (boatCollider == null)
        {
            Debug.LogError("Ship needs a collider!");
        }
    }

    void FixedUpdate()
    {
        // Calculate submerged volume (APPROXIMATION)
        float submergedVolume = CalculateSubmergedVolume();

        // Calculate buoyant force
        float buoyantForceMagnitude = submergedVolume * waterDensity * Physics.gravity.magnitude;
        Vector3 buoyantForce = Vector3.up * buoyantForceMagnitude;

        // Apply force at the center of buoyancy (approximation)
        Vector3 centerOfBuoyancy = boatCollider.bounds.center;
        rb.AddForceAtPosition(buoyantForce, centerOfBuoyancy, ForceMode.Force);

        // Wave motion (optional, add this back if desired, but base it on the buoyantForce)
        // Example: Apply a sinusoidal wave force to simulate wave motion
        // float waveForce = Mathf.Sin(Time.time) * waveAmplitude;
        // rb.AddForceAtPosition(Vector3.up * waveForce, centerOfBuoyancy, ForceMode.Force);
    }

    float CalculateSubmergedVolume()
    {
        // Simplified Submerged Volume Calculation (Approximation):
        float volume = boatCollider.bounds.size.x * boatCollider.bounds.size.y * boatCollider.bounds.size.z;
        float submergedHeight = Mathf.Max(0, waterLevelY - boatCollider.bounds.min.y);
        boatSubmergedPercentage = Mathf.Clamp01(submergedHeight / boatCollider.bounds.size.y); // Percentage to 0-1
        return volume * boatSubmergedPercentage;
    }

    public void SetWaterObject(float waterLevelY)
    {
        this.waterLevelY = waterLevelY;
    }
}
