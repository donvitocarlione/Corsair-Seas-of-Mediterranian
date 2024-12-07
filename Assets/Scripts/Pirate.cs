using UnityEngine;
using System.Collections.Generic;

public class Pirate : SeaEntityBase
{
    protected List<Ship> ownedShips = new List<Ship>();
    public float reputation = 50f;
    public float wealth = 1000f;
    
    protected virtual void Start()
    {
        // Register with FactionManager's faction data
        var factionData = FactionManager.Instance.GetFactionData(Faction);
        if (factionData != null)
        {
            if (!factionData.pirates.Contains(this))
            {
                factionData.pirates.Add(this);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        // Unregister from FactionManager's faction data
        var factionData = FactionManager.Instance.GetFactionData(Faction);
        if (factionData != null)
        {
            factionData.pirates.Remove(this);
        }
    }
    
    public virtual void AddShip(Ship ship)
    {
        if (!ownedShips.Contains(ship))
        {
            ownedShips.Add(ship);
            ship.SetOwner(this);
        }
    }

    public virtual void RemoveShip(Ship ship)
    {
        if (ownedShips.Contains(ship))
        {
            ownedShips.Remove(ship);
            if (ship.owner == this)
            {
                ship.SetOwner(null);
            }
        }
    }

    public virtual void SelectShip(Ship ship)
    {
        // Base implementation - can be overridden by Player
        if (ownedShips.Contains(ship))
        {
            foreach (var ownedShip in ownedShips)
            {
                if (ownedShip != ship && ownedShip.IsSelected)
                {
                    ownedShip.Deselect();
                }
            }
            ship.Select();
        }
    }

    public List<Ship> GetOwnedShips()
    {
        return new List<Ship>(ownedShips);
    }

    protected override void OnFactionChanged()
    {
        base.OnFactionChanged();
        // Update faction for all owned ships
        foreach (var ship in ownedShips)
        {
            ship.SetFaction(Faction);
        }

        // Re-register with new faction
        var factionData = FactionManager.Instance.GetFactionData(Faction);
        if (factionData != null && !factionData.pirates.Contains(this))
        {
            factionData.pirates.Add(this);
        }
    }
}