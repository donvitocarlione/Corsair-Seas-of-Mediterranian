using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Game/Pirate")]
public class Pirate : SeaEntityBase
{
    protected List<Ship> ownedShips;
    [SerializeField, Range(0f, 100f), Tooltip("Pirate's reputation affects trading and diplomacy")]
    public float reputation = 50f;
    [SerializeField, Min(0f), Tooltip("Current wealth in gold coins")]
    public float wealth = 1000f;

    private bool isInitialized;
    private const float MIN_REPUTATION = 0f;
    private const float MAX_REPUTATION = 100f;

    protected virtual void Awake()
    {
        ownedShips = new List<Ship>();
    }
    
    protected virtual void Start()
    {
        // Only register if not the player (player will be registered by ShipManager)
        if (!(this is Player))
        {
            RegisterWithFaction();
        }
    }

    protected virtual void OnDestroy()
    {
        if (isInitialized)
        {
            UnregisterFromFaction();
        }

        // Clean up ships
        if (ownedShips != null)
        {
            foreach (var ship in ownedShips.ToArray())
            {
                if (ship != null)
                {
                    RemoveShip(ship);
                }
            }
            ownedShips.Clear();
        }
    }

    public void ModifyReputation(float amount)
    {
        reputation = Mathf.Clamp(reputation + amount, MIN_REPUTATION, MAX_REPUTATION);
    }

    public void ModifyWealth(float amount)
    {
        wealth = Mathf.Max(0f, wealth + amount);
    }

    public override void SetFaction(FactionType faction)
    {
        if (!isInitialized || faction != Faction)
        {
            if (isInitialized)
            {
                UnregisterFromFaction();
            }
            
            base.SetFaction(faction);
            RegisterWithFaction();
            isInitialized = true;
        }
    }

    private void RegisterWithFaction()
    {
        if (FactionManager.Instance == null)
        {
            Debug.LogError("FactionManager instance not found!");
            return;
        }

        var factionData = FactionManager.Instance.GetFactionData(Faction);
        if (factionData == null)
        {
            Debug.LogError($"No faction data found for faction {Faction}");
            return;
        }

        if (!factionData.pirates.Contains(this))
        {
            factionData.pirates.Add(this);
            Debug.Log($"Registered pirate with faction {Faction}");
        }
    }

    private void UnregisterFromFaction()
    {
        if (FactionManager.Instance == null) return;

        var factionData = FactionManager.Instance.GetFactionData(Faction);
        if (factionData != null && factionData.pirates.Contains(this))
        {
            factionData.pirates.Remove(this);
            Debug.Log($"Unregistered pirate from faction {Faction}");
        }
    }
    
    public virtual void AddShip(Ship ship)
    {
        if (ship == null)
        {
            Debug.LogError("Attempting to add a null ship!");
            return;
        }

        if (!ownedShips.Contains(ship))
        {
            ownedShips.Add(ship);
            ship.SetOwner(this);
            ship.SetFaction(Faction); // Ensure ship has same faction
            Debug.Log($"Added ship {ship.shipName} to {GetType().Name}'s fleet");
        }
    }

    public virtual void RemoveShip(Ship ship)
    {
        if (ship == null)
        {
            Debug.LogError("Attempting to remove a null ship!");
            return;
        }

        if (ownedShips.Contains(ship))
        {
            ownedShips.Remove(ship);
            if (ship.owner == this)
            {
                ship.SetOwner(null);
            }
            Debug.Log($"Removed ship {ship.shipName} from {GetType().Name}'s fleet");
        }
    }

    public virtual void SelectShip(Ship ship)
    {
        if (ship == null)
        {
            Debug.LogError("Attempting to select a null ship!");
            return;
        }

        if (ownedShips.Contains(ship))
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
        Debug.Log($"{GetType().Name}'s faction changed to {Faction}");

        if (ownedShips == null) return;
        
        // Update faction for all owned ships
        foreach (var ship in ownedShips.ToArray()) // Use ToArray to avoid modification during enumeration
        {
            if (ship != null)
            {
                ship.SetFaction(Faction);
            }
        }
    }
}
