using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float baseTurnSpeed = 90f;
    public float acceleration = 1f;
    public float stoppingDistance = 1f;

    [Header("Movement Modifiers")]
    public float speedMultiplier = 1f;
    public float turnSpeedMultiplier = 1f;
    public bool isMoving { get; private set; }

    private Rigidbody rb;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    private Quaternion targetRotation;
    private Vector2 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Ship needs a Rigidbody!");
            enabled = false;
            return;
        }

        InitializePhysics();
    }

    private void InitializePhysics()
    {
        rb.useGravity = true;
        rb.mass = 1000f;
        rb.drag = 1f;
        rb.angularDrag = 2f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationZ;
    }

    public void SetMovementInput(float horizontal, float vertical)
    {
        movementInput = new Vector2(horizontal, vertical);
        
        if (movementInput.magnitude > 0)
        {
            // Convert input to world space movement
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
            isMoving = true;

            // Calculate rotation based on input direction
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // Set target position some distance ahead in the movement direction
            targetPosition = transform.position + moveDirection * 10f;
        }
        else
        {
            ClearTarget();
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

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        targetPosition.y = transform.position.y;
        isMoving = true;

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
    }

    private void RotateTowardsTarget()
    {
        float currentTurnSpeed = baseTurnSpeed * turnSpeedMultiplier;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
            currentTurnSpeed * Time.fixedDeltaTime);
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

        float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
        float angleMult = Mathf.Clamp01(1f - (angleToTarget / 90f));

        float currentSpeed = baseSpeed * speedMultiplier;
        Vector3 moveDirection = transform.forward;
        Vector3 targetVelocity = moveDirection * currentSpeed * angleMult;

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 
            acceleration * Time.fixedDeltaTime);
        rb.velocity = currentVelocity;
    }
}
