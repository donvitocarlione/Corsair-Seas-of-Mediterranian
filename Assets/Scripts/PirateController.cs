using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Pirate))]
public class PirateController : MonoBehaviour
{
    private Pirate pirate;
    private ShipManager shipManager;

    [Header("AI Settings")]
    public float decisionInterval = 1f;
    public float detectionRange = 50f;
    public LayerMask shipLayer;

    [Header("Debug")]
    public bool showDebugGizmos = true;
    
    private float nextDecisionTime;

    private void Start()
    {
        // Get required components
        pirate = GetComponent<Pirate>();
        shipManager = FindObjectOfType<ShipManager>();

        if (pirate == null)
        {
            Debug.LogError("PirateController requires a Pirate component!");
            enabled = false;
            return;
        }

        // Initialize decision timer
        nextDecisionTime = Time.time + Random.Range(0f, decisionInterval);
    }

    private void Update()
    {
        if (Time.time >= nextDecisionTime)
        {
            MakeDecisions();
            nextDecisionTime = Time.time + decisionInterval;
        }
    }

    private void MakeDecisions()
    {
        // If no ship is selected, try to select best ship
        if (pirate.selectedShip == null && pirate.ships.Count > 0)
        {
            SelectBestShip();
        }

        // If we have a selected ship, make decisions based on current situation
        if (pirate.selectedShip != null)
        {
            EvaluateCurrentSituation();
        }
    }

    private void SelectBestShip()
    {
        Ship bestShip = null;
        float bestCompatibility = 0f;

        foreach (Ship ship in pirate.ships)
        {
            float compatibility = pirate.GetShipCompatibility(ship);
            if (compatibility > bestCompatibility)
            {
                bestCompatibility = compatibility;
                bestShip = ship;
            }
        }

        if (bestShip != null)
        {
            pirate.SelectShip(bestShip);
            Debug.Log($"{pirate.pirateName} selected {bestShip.shipName} (Compatibility: {bestCompatibility})");
        }
    }

    private void EvaluateCurrentSituation()
    {
        // Find nearby ships
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, detectionRange, shipLayer);
        List<Ship> nearbyShips = new List<Ship>();

        foreach (Collider col in nearbyColliders)
        {
            Ship ship = col.GetComponent<Ship>();
            if (ship != null && ship != pirate.selectedShip)
            {
                nearbyShips.Add(ship);
            }
        }

        // Analyze situation based on nearby ships
        foreach (Ship nearbyShip in nearbyShips)
        {
            // Check if ship belongs to a hostile faction
            if (IsHostileFaction(nearbyShip.faction))
            {
                // Evaluate combat situation
                EvaluateCombatSituation(nearbyShip);
            }
        }
    }

    private bool IsHostileFaction(FactionType otherFaction)
    {
        // Implement faction hostility check based on your game's diplomacy system
        // For now, consider all non-allied factions as potentially hostile
        return otherFaction != pirate.faction?.factionType;
    }

    private void EvaluateCombatSituation(Ship enemyShip)
    {
        Ship ourShip = pirate.selectedShip;

        // Simple combat evaluation
        float ourStrength = ourShip.currentAttack * ourShip.currentDefense * (ourShip.health / 100f);
        float enemyStrength = enemyShip.currentAttack * enemyShip.currentDefense * (enemyShip.health / 100f);

        float strengthRatio = ourStrength / enemyStrength;

        // Log the evaluation for debugging
        Debug.Log($"{pirate.pirateName} evaluated combat: Our strength {ourStrength:F1} vs Enemy strength {enemyStrength:F1} (Ratio: {strengthRatio:F2})");
    }

    private void OnDrawGizmos()
    {
        if (showDebugGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }
}
