using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour
{
    public string pirateName;
    public List<Ship> ships = new List<Ship>();
    public Faction faction;
    public Ship selectedShip;

    public float combatSkill = 1f;
    public float navigationSkill = 1f;
    public float tradingSkill = 1f;

    private void Start()
    {
        if (string.IsNullOrEmpty(pirateName))
        {
            GenerateRandomName();
        }
    }

    private void GenerateRandomName()
    {
        string[] titles = { "Captain", "Corsair", "Pirate", "Buccaneer" };
        string[] firstNames = { "Black", "Red", "Mad", "Salty", "Iron", "Gold" };
        string[] lastNames = { "Beard", "Hook", "Eye", "Dog", "Hand", "Heart" };
        
        pirateName = $"{titles[Random.Range(0, titles.Length)]} {firstNames[Random.Range(0, firstNames.Length)]} {lastNames[Random.Range(0, lastNames.Length)]}";
    }

    public virtual void AddShip(Ship ship)
    {
        if (!ships.Contains(ship))
        {
            ships.Add(ship);
            ship.SetOwner(this);
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
            ship.ResetStats();
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

    protected virtual void ApplySkillsToShip(Ship ship)
    {
        if (ship != null)
        {
            if (ship.isCombatShip)
            {
                ship.currentAttack *= combatSkill;
                ship.currentDefense *= combatSkill;
            }

            ShipMovement movement = ship.GetComponent<ShipMovement>();
            if (movement != null)
            {
                movement.speedMultiplier *= navigationSkill;
            }

            ship.maxCargo *= (1f + (tradingSkill - 1f) * 0.5f);
        }
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

    public virtual void SetFaction(Faction newFaction)
    {
        faction = newFaction;
    }
}
