using UnityEngine;
using System.Collections.Generic;

public class AIShipController : MonoBehaviour
{
    public Ship ship;
    public FactionManager factionManager;
    public DiplomacySystem diplomacySystem;
    private Ship targetShip;
    
    [Header("AI Settings")]
    public float patrolRadius = 50f;         // How far the ship will patrol
    public float updateTargetInterval = 2f;  // How often to update target/decision
    public float engageDistance = 20f;       // Distance at which to engage enemies
    public float fleeHealthThreshold = 0.3f; // Percentage of health to flee at
    
    private Vector3 patrolCenter;
    private Vector3 currentPatrolPoint;
    private float nextUpdateTime;
    private ShipMovement shipMovement;
    private AIState currentState = AIState.Patrolling;
    
    private enum AIState
    {
        Patrolling,
        Pursuing,
        Engaging,
        Fleeing
    }

    void Start()
    {
        Initialize(ship, diplomacySystem);
        shipMovement = GetComponent<ShipMovement>();
        patrolCenter = transform.position; // Use spawn position as patrol center
        SetNewPatrolPoint();
    }

    public void Initialize(Ship ship, DiplomacySystem diplomacySystem)
    {
        this.ship = ship;
        if (this.ship == null)
        {
            Debug.LogError("Ship component not found on this GameObject.");
            return;
        }

        factionManager = FindObjectOfType<FactionManager>();
        if (factionManager == null)
        {
            Debug.LogError("FactionManager not found in the scene.");
        }

        this.diplomacySystem = diplomacySystem;
        if (this.diplomacySystem == null)
        {
            Debug.LogError("DiplomacySystem not found in the scene.");
        }
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateAIState();
            nextUpdateTime = Time.time + updateTargetInterval;
        }

        ExecuteCurrentState();
    }

    void UpdateAIState()
    {
        // Check health for fleeing
        if (ship.health / 100f <= fleeHealthThreshold && currentState != AIState.Fleeing)
        {
            currentState = AIState.Fleeing;
            return;
        }

        // Look for enemies if not fleeing
        if (currentState != AIState.Fleeing)
        {
            Ship nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                targetShip = nearestEnemy;
                float distanceToTarget = Vector3.Distance(transform.position, targetShip.transform.position);
                
                if (distanceToTarget <= engageDistance)
                    currentState = AIState.Engaging;
                else
                    currentState = AIState.Pursuing;
            }
            else if (currentState != AIState.Patrolling)
            {
                currentState = AIState.Patrolling;
                SetNewPatrolPoint();
            }
        }
    }

    void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case AIState.Patrolling:
                ExecutePatrolling();
                break;

            case AIState.Pursuing:
                if (targetShip != null)
                    shipMovement.SetTargetPosition(targetShip.transform.position);
                break;

            case AIState.Engaging:
                if (targetShip != null)
                {
                    Vector3 directionToTarget = (targetShip.transform.position - transform.position).normalized;
                    Vector3 engagePosition = targetShip.transform.position - directionToTarget * (engageDistance * 0.8f);
                    shipMovement.SetTargetPosition(engagePosition);
                    // TODO: Add combat logic here
                }
                break;

            case AIState.Fleeing:
                ExecuteFleeing();
                break;
        }
    }

    void ExecutePatrolling()
    {
        if (Vector3.Distance(transform.position, currentPatrolPoint) < 5f)
        {
            SetNewPatrolPoint();
        }
        shipMovement.SetTargetPosition(currentPatrolPoint);
    }

    void ExecuteFleeing()
    {
        if (targetShip != null)
        {
            Vector3 fleeDirection = transform.position - targetShip.transform.position;
            Vector3 fleePosition = transform.position + fleeDirection.normalized * patrolRadius;
            shipMovement.SetTargetPosition(fleePosition);
        }
        else
        {
            currentState = AIState.Patrolling;
        }
    }

    void SetNewPatrolPoint()
    {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(patrolRadius * 0.3f, patrolRadius);
        
        Vector3 offset = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward * randomDistance;
        currentPatrolPoint = patrolCenter + offset;
        currentPatrolPoint.y = transform.position.y; // Keep same height
    }

    Ship FindNearestEnemy()
    {
        if (diplomacySystem == null || factionManager == null) return null;

        Ship nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (KeyValuePair<FactionType, FactionData> factionPair in factionManager.factions)
        {
            List<Ship> factionShips = factionManager.GetFactionShips(factionPair.Key);
            foreach (Ship otherShip in factionShips)
            {
                if (otherShip != this.ship && otherShip.gameObject.activeInHierarchy)
                {
                    DiplomacySystem.Relationship relationship = 
                        diplomacySystem.GetRelationship(this.ship.faction, otherShip.faction);
                    
                    if (relationship == DiplomacySystem.Relationship.Hostile)
                    {
                        float distance = Vector3.Distance(transform.position, otherShip.transform.position);
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestEnemy = otherShip;
                        }
                    }
                }
            }
        }

        return nearestEnemy;
    }
}