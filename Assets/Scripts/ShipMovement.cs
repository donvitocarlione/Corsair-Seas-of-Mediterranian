using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float floatForce = 10f; // Force applied to float the ship
    public float airDrag = 1f; // Drag coefficient for air
    public float waterDrag = 10f; // Water drag coefficient
    public float waterLevel = 0f; // Y position of the water level
    public float speed = 5f; // Speed of the ship
    public float acceleration = 1f; // Acceleration of the ship

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isMoving;
    private Vector3 currentVelocity;
    private float density = 1000f;  // Density of the ship (kg/m³) - Adjust this value
    private float shipVolume;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Ship needs a Rigidbody!");
            enabled = false; // Disable this script if no rigidbody is found
            return;
        }

        rb.useGravity = true;
        // Calculate the volume, you'll probably need to find a suitable method for your ship's model
        // Example: using a bounding box
        shipVolume = GetComponent<Collider>().bounds.size.x * GetComponent<Collider>().bounds.size.y * GetComponent<Collider>().bounds.size.z;
        rb.mass = shipVolume * density;
        rb.linearDamping = airDrag;
        rb.angularDamping = 2f;
        // Constrain rotation to only Y-axis
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    public void ClearTarget()
    {
        targetPosition = Vector3.zero;
        isMoving = false;
        currentVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (isMoving) MoveTowardsTarget();
        ApplyFloatForce();
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        currentVelocity = Vector3.MoveTowards(currentVelocity, direction * speed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = currentVelocity;

        // Check if the ship has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            currentVelocity = Vector3.zero;
        }
    }

    private void ApplyFloatForce()
    {
        float distanceToWater = transform.position.y - waterLevel;

        if (distanceToWater < 0)
        {
            Vector3 floatForceVector = Vector3.up * -distanceToWater * floatForce;
            rb.AddForce(floatForceVector, ForceMode.Acceleration); // Use ForceMode.Acceleration

            // Add water resistance force
            Vector3 velocityInWater = rb.linearVelocity;
            velocityInWater.y = 0; // Ignore vertical velocity
            rb.AddForce(-velocityInWater.normalized * velocityInWater.magnitude * waterDrag, ForceMode.Acceleration);
        }
    }
}
