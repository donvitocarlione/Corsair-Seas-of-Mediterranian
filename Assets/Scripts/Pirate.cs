using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour
{
    [Header("Basic Info")]
    public string pirateName;
    public List<Ship> ships = new List<Ship>();
    public Faction faction;
    public float reputation;
    public int gold;
    public Ship selectedShip;

    [Header("Ship Management")]
    public int maxShipCount = 3;
    public float preferredShipSize = 0.5f;
    public bool prefersCombatShips = true;

    [Header("Skills")]
    public float combatSkill = 1f;
    public float navigationSkill = 1f;
    public float tradingSkill = 1f;
    public int captainLevel = 1;

    private void Start()
    {
        if (string.IsNullOrEmpty(pirateName))
        {
            GenerateRandomName();
            InitializeRandomStats();
        }
    }

    private void GenerateRandomName()
    {
        string[] titles = { "Captain", "Corsair", "Pirate", "Buccaneer" };
        string[] firstNames = { "Black", "Red", "Mad", "Salty", "Iron", "Gold" };
        string[] lastNames = { "Beard", "Hook", "Eye", "Dog", "Hand", "Heart" };
        
        pirateName = $"{titles[Random.Range(0, titles.Length)]} {firstNames[Random.Range(0, firstNames.Length)]} {lastNames[Random.Range(0, lastNames.Length)]}";
    }

    private void InitializeRandomStats()
    {
        captainLevel = Random.Range(1, 6);
        combatSkill = Random.Range(0.5f, 2f);
        navigationSkill = Random.Range(0.5f, 2f);
        tradingSkill = Random.Range(0.5f, 2f);
        preferredShipSize = Random.value;
        prefersCombatShips = Random.value > 0.5f;
        maxShipCount = Random.Range(1, 4);
        reputation = Random.Range(-50f, 50f);
        gold = Random.Range(100, 1000);
    }

    public virtual void AddShip(Ship ship)
    {
        if (!ships.Contains(ship) && ships.Count < maxShipCount)
        {
            ships.Add(ship);
            ship.SetOwner(this);
            
            // Apply pirate skills to ship
            ApplySkillsToShip(ship);
            
            if (ships.Count == 1)
            {
                SelectShip(ship);
            }
        }
    }

    public virtual void RemoveShip(Ship ship)
    {
        if (ships.Contains(ship))
        {
            ships.Remove(ship);
            RemoveSkillsFromShip(ship);
            ship.SetOwner(null);
            
            if (selectedShip == ship)
            {
                selectedShip = ships.Count > 0 ? ships[0] : null;
                if (selectedShip != null)
                {
                    SelectShip(selectedShip);
                }
            }
        }
    }

    private void ApplySkillsToShip(Ship ship)
    {
        if (ship != null)
        {
            // Apply combat bonus
            if (ship.isCombatShip)
            {
                ship.currentAttack *= combatSkill;
                ship.currentDefense *= combatSkill;
            }

            // Apply navigation bonus to movement
            ShipMovement movement = ship.GetComponent<ShipMovement>();
            if (movement != null)
            {
                movement.speedMultiplier *= navigationSkill;
            }

            // Apply trading bonus
            ship.maxCargo *= (1f + (tradingSkill - 1f) * 0.5f);
        }
    }

    private void RemoveSkillsFromShip(Ship ship)
    {
        if (ship != null)
        {
            ship.ResetStats();
        }
    }

    public float GetShipCompatibility(Ship ship)
    {
        if (ship == null) return 0f;

        float compatibility = 1f;

        // Size preference (0-1 scale)
        float sizeDiff = 1f - Mathf.Abs(preferredShipSize - ship.shipSize);
        compatibility *= (1f + sizeDiff);

        // Combat preference
        if (prefersCombatShips && ship.isCombatShip)
            compatibility *= 1.5f;
        else if (!prefersCombatShips && !ship.isCombatShip)
            compatibility *= 1.5f;

        // Skill matching
        if (ship.isCombatShip)
            compatibility *= combatSkill;
        else
            compatibility *= tradingSkill;

        // Captain level bonus
        compatibility *= (1f + (captainLevel - 1) * 0.1f);

        return compatibility;
    }

    public virtual bool SelectShip(Ship ship)
    {
        if (ships.Contains(ship))
        {
            if (selectedShip != null)
            {
                selectedShip.Deselect();
            }
            selectedShip = ship;
            return ship.Select();
        }
        return false;
    }

    public void SetFaction(Faction newFaction)
    {
        if (faction != null)
        {
            faction.RemoveMember(this);
        }
        faction = newFaction;
    }

    public void JoinFaction(Faction newFaction)
    {
        if (newFaction != null)
        {
            newFaction.AddMember(this);
        }
    }

    public void LeaveFaction()
    {
        if (faction != null)
        {
            faction.RemoveMember(this);
        }
    }
}
