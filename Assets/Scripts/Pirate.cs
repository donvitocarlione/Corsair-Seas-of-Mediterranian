using UnityEngine;
using System.Collections.Generic;

public class Pirate : MonoBehaviour
{
    protected List<Ship> ships = new List<Ship>();
    protected FactionType faction;

    public virtual void SetFaction(FactionType newFaction)
    {
        faction = newFaction;
        // Update faction for all ships
        foreach (Ship ship in ships)
        {
            if (ship != null)
            {
                ship.faction = newFaction;
            }
        }
    }

    public virtual void AddShip(Ship ship)
    {
        if (ship != null)
        {
            ships.Add(ship);
            ship.SetOwner(this);
            ship.faction = faction;
        }
    }

    public virtual void RemoveShip(Ship ship)
    {
        if (ships.Contains(ship))
        {
            ships.Remove(ship);
        }
    }

    public virtual void SelectShip(Ship ship)
    {
        // Base implementation does nothing
    }

    public virtual List<Ship> GetShips()
    {
        return ships;
    }

    public FactionType GetFaction()
    {
        return faction;
    }
}
