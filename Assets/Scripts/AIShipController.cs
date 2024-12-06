using UnityEngine;
using System.Collections.Generic;

public class AIShipController : MonoBehaviour
{
    public Ship ship;
    public FactionManager factionManager;
    public DiplomacySystem diplomacySystem;
    private Ship targetShip;

    void Start()
    {
        Initialize(ship, diplomacySystem);
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
        if (targetShip == null || !targetShip.gameObject.activeInHierarchy)
        {
            FindTarget();
        }

        if (targetShip != null)
        {
            this.ship.MoveTo(targetShip.transform.position);
        }
    }

    void FindTarget()
    {
        if (diplomacySystem == null || factionManager == null) return;

        foreach (KeyValuePair<FactionType, FactionData> factionPair in factionManager.factions)
        {
            FactionType factionType = factionPair.Key; // Access the FactionType from the Key

            List<Ship> factionShips = factionManager.GetFactionShips(factionType);
            foreach (Ship otherShip in factionShips)
            {
                if (otherShip != this.ship)
                {
                    DiplomacySystem.Relationship relationship = diplomacySystem.GetRelationship(this.ship.faction, otherShip.faction);
                    if (relationship == DiplomacySystem.Relationship.Hostile)
                    {
                        targetShip = otherShip;
                        break; // Target the first enemy
                    }
                }
            }
            if (targetShip != null) break; // Break early if an enemy was found
        }

        if (targetShip == null)
        {
            Debug.Log("No enemy ships found to target.");
        }
    }
}
