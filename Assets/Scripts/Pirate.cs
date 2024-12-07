using UnityEngine;
using System.Collections.Generic;

public class Pirate : SeaEntityBase
{
    protected List<Ship> ownedShips;
    public float reputation = 50f;
    public float wealth = 1000f;

    protected virtual void Awake()
    {
        ownedShips = new List<Ship>();
    }
    
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
                Debug.Log($"Registered pirate with faction {Faction}");
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
                Debug.Log($"Unregistered pirate from faction {Faction}");
            }
        }
    }
    
    public virtual void AddShip(Ship ship)
    {
        if (ship != null && !ownedShips.Contains(ship))
        {
            ownedShips.Add(ship);
            ship.SetOwner(this);
            Debug.Log($"Added ship {ship.shipName} to pirate's fleet");
        }
    }

    public virtual void RemoveShip(Ship ship)
    {
        if (ship != null && ownedShips.Contains(ship))
        {
            ownedShips.Remove(ship);
            if (ship.owner == this)
            {
                ship.SetOwner(null);
            }
            Debug.Log($"Removed ship {ship.shipName} from pirate's fleet");
        }
    }

    public virtual void SelectShip(Ship ship)
    {
        if (ship != null && ownedShips.Contains(ship))
        {
            foreach (var ownedShip in ownedShips)
            {
                if (ownedShip != null && ownedShip != ship && ownedShip.IsSelected)
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
        Debug.Log($"Pirate faction changed to {Faction}");

        // Only proceed if we have ships to update
        if (ownedShips == null) return;
        
        // Update faction for all owned ships
        foreach (var ship in ownedShips.ToArray()) // Use ToArray to avoid modification during enumeration
        {
            if (ship != null)
            {
                ship.SetFaction(Faction);
            }
        }

        // Re-register with new faction if FactionManager is available
        if (FactionManager.Instance != null)
        {
            UnregisterFromFaction(); // Unregister from old faction
            RegisterWithFaction();    // Register with new faction
        }
    }
}
