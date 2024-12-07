using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    public float waterDensity = 1000f;
    public float waterLevelY = 0f;
    public float buoyancyForce = 15f; // Added multiplier for easy tuning
    public float waterDrag = 2f;
    public float waterAngularDrag = 2f;

    [Header("Debug Info")]
    public float boatSubmergedPercentage = 0f;
    public bool isInWater;

    private Rigidbody rb;
    private Collider boatCollider;
    private float initialDrag;
    private float initialAngularDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boatCollider = GetComponent<Collider>();

        if (rb == null)
        {
            Debug.LogError("Ship needs a Rigidbody!");
            enabled = false;
            return;
        }

        if (boatCollider == null)
        {
            Debug.LogError("Ship needs a collider!");
            enabled = false;
            return;
        }

        // Store initial values
        initialDrag = rb.drag;
        initialAngularDrag = rb.angularDrag;

        // Set recommended Rigidbody settings
        rb.mass = 1000f; // 1 ton
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }

    void FixedUpdate()
    {
        // Calculate submerged percentage
        boatSubmergedPercentage = CalculateSubmergedPercentage();
        isInWater = boatSubmergedPercentage > 0;

        if (isInWater)
        {
            // Apply buoyancy force
            float forceMagnitude = buoyancyForce * boatSubmergedPercentage * waterDensity * -Physics.gravity.y;
            Vector3 buoyantForce = Vector3.up * forceMagnitude;

            // Apply force at the center of buoyancy (slightly below center for stability)
            Vector3 centerOfBuoyancy = boatCollider.bounds.center;
            centerOfBuoyancy.y = Mathf.Lerp(boatCollider.bounds.min.y, boatCollider.bounds.center.y, 0.5f);
            rb.AddForceAtPosition(buoyantForce, centerOfBuoyancy, ForceMode.Force);

            // Apply water resistance
            rb.drag = waterDrag;
            rb.angularDrag = waterAngularDrag;
        }
        else
        {
            // Reset drag when out of water
            rb.drag = initialDrag;
            rb.angularDrag = initialAngularDrag;
        }
    }

    float CalculateSubmergedPercentage()
    {
        float shipBottom = boatCollider.bounds.min.y;
        float shipHeight = boatCollider.bounds.size.y;
        float submergedHeight = Mathf.Max(0, waterLevelY - shipBottom);
        return Mathf.Clamp01(submergedHeight / shipHeight);
    }

    public void SetWaterLevel(float newWaterLevelY)
    {
        waterLevelY = newWaterLevelY;
    }

    void OnDrawGizmosSelected()
    {
        // Draw water level
        if (boatCollider != null)
        {
            Gizmos.color = Color.blue;
            Vector3 center = boatCollider.bounds.center;
            Vector3 size = boatCollider.bounds.size;
            Vector3 waterLineCenter = new Vector3(center.x, waterLevelY, center.z);
            Gizmos.DrawWireCube(waterLineCenter, new Vector3(size.x, 0.1f, size.z));
        }
    }
}
