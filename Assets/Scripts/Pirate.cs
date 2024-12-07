using UnityEngine;
using System.Collections.Generic;

public class Pirate : SeaEntityBase
{
    protected List<Ship> ownedShips = new List<Ship>();
    public float reputation = 50f;
    public float wealth = 1000f;
    
    protected virtual void Start()
    {
        RegisterWithFaction();
    }

    protected virtual void OnDestroy()
    {
        UnregisterFromFaction();
    }

    private void RegisterWithFaction()
    {
        if (FactionManager.Instance != null)
        {
            var factionData = FactionManager.Instance.GetFactionData(Faction);
            if (factionData != null && !factionData.pirates.Contains(this))
            {
                factionData.pirates.Add(this);
            }
        }
    }

    private void UnregisterFromFaction()
    {
        if (FactionManager.Instance != null)
        {
            var factionData = FactionManager.Instance.GetFactionData(Faction);
            if (factionData != null)
            {
                factionData.pirates.Remove(this);
            }
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
        if (ownedShips != null)
        {
            foreach (var ship in ownedShips)
            {
                if (ship != null)
                {
                    ship.SetFaction(Faction);
                }
            }
        }

        // Re-register with new faction
        if (FactionManager.Instance != null)
        {
            UnregisterFromFaction(); // Unregister from old faction
            RegisterWithFaction();    // Register with new faction
        }
    }
}
