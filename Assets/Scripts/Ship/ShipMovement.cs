using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [Header("References")]
    public Ship ship;
    private Rigidbody rb;

    [Header("Movement")]
    public float currentSpeed;
    public float targetSpeed;
    public float currentRotation;
    public float targetRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Update speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, ship.GetAcceleration() * Time.fixedDeltaTime);
        
        // Apply movement
        Vector3 movement = transform.forward * currentSpeed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

        // Update rotation
        currentRotation = Mathf.MoveTowards(currentRotation, targetRotation, ship.GetSteeringSpeed() * Time.fixedDeltaTime);
        
        // Apply rotation
        Quaternion rotation = Quaternion.Euler(0f, currentRotation, 0f);
        rb.MoveRotation(rotation);
    }

    public void SetTargetSpeed(float speed)
    {
        targetSpeed = Mathf.Clamp(speed, 0f, ship.GetSpeed());
    }

    public void SetTargetRotation(float rotation)
    {
        targetRotation = rotation;
    }

    public void Stop()
    {
        targetSpeed = 0f;
    }
}
