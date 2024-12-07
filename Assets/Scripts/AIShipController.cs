using UnityEngine;

public class AIShipController : MonoBehaviour
{
    public Ship controlledShip;
    public DiplomacySystem diplomacySystem;
    public float decisionInterval = 2f;
    public float patrolRadius = 100f;
    public float detectionRange = 50f;

    private ShipMovement movement;
    private Vector3 homePosition;
    private float nextDecisionTime;
    private Ship targetShip;

    public void Initialize(Ship ship, DiplomacySystem diplomacy)
    {
        controlledShip = ship;
        diplomacySystem = diplomacy;
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
        if (diplomacySystem == null)
        {
            // Try to find DiplomacySystem if not set
            diplomacySystem = Object.FindAnyObjectByType<DiplomacySystem>();
            if (diplomacySystem == null)
                return;
        }

        // Look for potential targets
        targetShip = FindNearestHostileShip();

        if (targetShip != null)
        {
            // Move to intercept target
            movement.SetTargetPosition(targetShip.transform.position);
        }
        else
        {
            // Continue patrolling
            Patrol();
        }
    }

    private void Patrol()
    {
        if (!movement.isMoving)
        {
            // Generate new patrol point
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            Vector3 newPosition = homePosition + new Vector3(randomCircle.x, 0, randomCircle.y);
            movement.SetTargetPosition(newPosition);
        }
    }

    private Ship FindNearestHostileShip()
    {
        Ship nearest = null;
        float closestDistance = detectionRange;

        // Find all ships in the scene
        foreach (Ship ship in FindObjectsByType<Ship>(FindObjectsSortMode.None))
        {
            if (ship == controlledShip || ship == null)
                continue;

            // Check if ship is hostile
            if (diplomacySystem.AreFactionsHostile(controlledShip.faction, ship.faction))
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
