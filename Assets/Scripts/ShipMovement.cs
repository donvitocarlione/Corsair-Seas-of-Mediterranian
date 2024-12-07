using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float floatForce = 10f;
    public float airDrag = 1f;
    public float waterDrag = 10f;
    public float waterLevel = 0f;
    public float speed = 5f;
    public float turnSpeed = 90f;
    public float acceleration = 1f;
    public float stoppingDistance = 1f;
    public float rotationDamping = 0.5f;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isMoving;
    private Vector3 currentVelocity;
    private float density = 1000f;
    private float shipVolume;
    private Quaternion targetRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Ship needs a Rigidbody!");
            enabled = false;
            return;
        }

        rb.useGravity = true;
        shipVolume = GetComponent<Collider>().bounds.size.x * 
                    GetComponent<Collider>().bounds.size.y * 
                    GetComponent<Collider>().bounds.size.z;
        rb.mass = shipVolume * density;
        rb.drag = airDrag;
        rb.angularDrag = 2f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationZ;
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        targetPosition.y = transform.position.y; // Keep same height
        isMoving = true;

        // Calculate target rotation immediately
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        if (directionToTarget != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0, targetAngle, 0);
        }
    }

    public void ClearTarget()
    {
        targetPosition = Vector3.zero;
        isMoving = false;
        currentVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (isMoving) 
        {
            RotateTowardsTarget();
            MoveTowardsTarget();
        }
        ApplyFloatForce();
    }

    private void RotateTowardsTarget()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
            turnSpeed * Time.fixedDeltaTime);
    }

    private void MoveTowardsTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget <= stoppingDistance)
        {
            isMoving = false;
            currentVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            return;
        }

        // Only move forward if we're mostly facing the right direction
        float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
        float speedMultiplier = Mathf.Clamp01(1f - (angleToTarget / 90f));

        // Use the ship's forward direction for movement
        Vector3 moveDirection = transform.forward;
        Vector3 targetVelocity = moveDirection * speed * speedMultiplier;

        // Smoothly interpolate current velocity
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 
            acceleration * Time.fixedDeltaTime);
        rb.velocity = currentVelocity;
    }

    private void ApplyFloatForce()
    {
        float distanceToWater = transform.position.y - waterLevel;

        if (distanceToWater < 0)
        {
            Vector3 floatForceVector = Vector3.up * -distanceToWater * floatForce;
            rb.AddForce(floatForceVector, ForceMode.Acceleration);

            // Add water resistance force
            Vector3 velocityInWater = rb.velocity;
            velocityInWater.y = 0; // Ignore vertical velocity
            rb.AddForce(-velocityInWater.normalized * velocityInWater.sqrMagnitude * 
                waterDrag, ForceMode.Acceleration);
        }
    }
}
