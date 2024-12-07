using UnityEngine;
using System.Collections.Generic;

public class Player : Pirate
{
    public string pirateName;
    public Ship selectedShip;

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
        base.SetFaction(newFaction);
        Debug.Log($"Player faction set to: {newFaction}");
    }

    public override void AddShip(Ship ship)
    {
        if (ship != null)
        {
            base.AddShip(ship);
            Debug.Log($"Added ship {ship.shipName} to player's fleet. Current ship count: {ships.Count}");
        }
    }

    public override void RemoveShip(Ship ship)
    {
        base.RemoveShip(ship);
        if (selectedShip == ship)
        {
            selectedShip = null;
        }
    }

    public override void SelectShip(Ship ship)
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
