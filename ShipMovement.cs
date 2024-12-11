using UnityEngine;

public enum ShipState
{
    Idle,
    Moving,
    Turning,
    Stopping
}

public class ShipMovement : MonoBehaviour
{
    [Header("Ship Characteristics")]
    public float maxSpeed = 5f;
    public float turnSpeed = 90f;
    
    [Header("Movement Settings")]
    public float acceleration = 1f;
    public float deceleration = 0.5f;
    public float stoppingDistance = 1f;
    public float minSpeedForTurning = 0.1f;
    
    [Header("Movement Modifiers")]
    public float speedMultiplier = 1f;
    public float turnSpeedMultiplier = 1f;
    
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private ShipState currentState = ShipState.Idle;
    private float currentSpeed = 0f;
    private bool _isMoving;

    // Public property to access movement state
    public bool IsMoving => _isMoving;
    public ShipState CurrentState => currentState;
    
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        targetPosition.y = transform.position.y;  // Maintain current height
        _isMoving = true;
        
        // Calculate rotation to face target
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0, targetAngle, 0);
            currentState = ShipState.Turning;
        }
    }
    
    public void StopMovement()
    {
        _isMoving = false;
        currentSpeed = 0f;
        currentState = ShipState.Stopping;
    }
    
    private void Update()
    {
        switch (currentState)
        {
            case ShipState.Turning:
                HandleRotation();
                break;
                
            case ShipState.Moving:
                HandleMovement();
                break;
                
            case ShipState.Stopping:
                HandleStopping();
                break;
        }
    }
    
    private void HandleRotation()
    {
        if (currentSpeed < minSpeedForTurning) 
        {
            currentSpeed += acceleration * Time.deltaTime;
            return;
        }
        
        float step = turnSpeed * turnSpeedMultiplier * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            currentState = ShipState.Moving;
        }
    }
    
    private void HandleMovement()
    {
        if (!_isMoving) return;
        
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        
        if (distanceToTarget <= stoppingDistance)
        {
            StopMovement();
            return;
        }
        
        // Accelerate/decelerate smoothly
        float targetSpeed = maxSpeed * speedMultiplier;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        
        // Move forward
        Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;
        newPosition.y = transform.position.y;  // Maintain height from buoyancy
        transform.position = newPosition;
    }
    
    private void HandleStopping()
    {
        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
            Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
            transform.position += movement;
        }
        else
        {
            currentState = ShipState.Idle;
        }
    }
    
    public void ApplyNavigationBonus(float bonus)
    {
        speedMultiplier = bonus;
        turnSpeedMultiplier = Mathf.Lerp(1f, bonus, 0.5f);
    }
    
    public void ResetNavigationBonus()
    {
        speedMultiplier = 1f;
        turnSpeedMultiplier = 1f;
    }
}