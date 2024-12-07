using UnityEngine;
using System.Collections.Generic;

public class Player : Pirate
{
    public string pirateName;
    public Ship selectedShip;
    public List<Ship> ships = new List<Ship>();
    private FactionType faction;

    void Start()
    {
        // Make sure the player's faction is set from ShipManager
        if (ShipManager.Instance != null)
        {
            SetFaction(ShipManager.Instance.PlayerFaction);
        }
    }

    public override void SetFaction(FactionType newFaction)
    {
        faction = newFaction;
        Debug.Log($"Player faction set to: {newFaction}");
        
        // Update faction for all existing ships
        foreach (Ship ship in ships)
        {
            if (ship != null)
            {
                ship.faction = newFaction;
            }
        }
    }

    public override void AddShip(Ship ship)
    {
        if (ship != null)
        {
            ships.Add(ship);
            ship.SetOwner(this);
            ship.faction = faction; // Ensure the ship has the correct faction
            Debug.Log($"Added ship {ship.shipName} to player's fleet. Current ship count: {ships.Count}");
        }
    }

    public override void RemoveShip(Ship ship)
    {
        if (ships.Contains(ship))
        {
            ships.Remove(ship);
            if (selectedShip == ship)
            {
                selectedShip = null;
            }
        }
    }

    public void SelectShip(Ship ship)
    {
        if (ships.Contains(ship))
        {
            if (selectedShip != null)
            {
                selectedShip.Deselect();
            }
            selectedShip = ship;
            selectedShip.Select();
        }
    }

    public override List<Ship> GetShips()
    {
        return ships;
    }
}
