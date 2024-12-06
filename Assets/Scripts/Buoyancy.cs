using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    public float waterLevel = 0f;
    public float floatForce = 15f;
    public float waterDrag = 2f;
    public float waterAngularDrag = 1f;

    private Rigidbody rb;
    private float originalDrag;
    private float originalAngularDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            originalDrag = rb.drag;
            originalAngularDrag = rb.angularDrag;
        }
        else
        {
            Debug.LogError("Rigidbody required for buoyancy!");
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        float distanceToWater = transform.position.y - waterLevel;
        bool isUnderwater = distanceToWater < 0;

        // Apply buoyancy force when underwater
        if (isUnderwater)
        {
            float displacementMultiplier = Mathf.Abs(distanceToWater);
            Vector3 buoyancyForce = Vector3.up * Mathf.Abs(Physics.gravity.y) * floatForce * displacementMultiplier;
            rb.AddForce(buoyancyForce, ForceMode.Acceleration);

            // Apply water resistance
            rb.drag = waterDrag;
            rb.angularDrag = waterAngularDrag;
        }
        else
        {
            // Reset drag when above water
            rb.drag = originalDrag;
            rb.angularDrag = originalAngularDrag;
        }
    }
}