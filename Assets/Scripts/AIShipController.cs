using UnityEngine;

public class AIShipController : MonoBehaviour
{
    public Ship controlledShip;
    public float decisionInterval = 2f;
    public float patrolRadius = 100f;
    public float detectionRange = 50f;

    private ShipMovement movement;
    private Vector3 homePosition;
    private float nextDecisionTime;
    private Ship targetShip;

    public void Initialize(Ship ship)
    {
        controlledShip = ship;
        movement = GetComponent<ShipMovement>();
        if (movement == null)
        {
            Debug.LogError("ShipMovement component missing!");
            enabled = false;
            return;
        }

        homePosition = transform.position;
        nextDecisionTime = Time.time + Random.Range(0f, decisionInterval);
    }

    void Update()
    {
        if (Time.time >= nextDecisionTime)
        {
            MakeDecisions();
            nextDecisionTime = Time.time + decisionInterval;
        }
    }

    private void MakeDecisions()
    {
        // Look for potential targets
        targetShip = FindNearestHostileShip();

        if (targetShip != null)
        {
            // Move to intercept target
            movement.SetTargetPosition(targetShip.transform.position);
        }
        else if (!movement.isMoving)
        {
            // Continue patrolling
            Patrol();
        }
    }

    private void Patrol()
    {
        // Generate new patrol point
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 newPosition = homePosition + new Vector3(randomCircle.x, 0, randomCircle.y);
        movement.SetTargetPosition(newPosition);
    }

    private Ship FindNearestHostileShip()
    {
        Ship nearest = null;
        float closestDistance = detectionRange;

        foreach (Ship ship in FindObjectsByType<Ship>(FindObjectsSortMode.None))
        {
            if (ship == controlledShip || ship == null)
                continue;

            // Check if ships are hostile according to diplomacy system
            if (DiplomacySystem.Instance.AreHostile(controlledShip.faction, ship.faction))
            {
                float distance = Vector3.Distance(transform.position, ship.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearest = ship;
                }
            }
        }

        return nearest;
    }
}
