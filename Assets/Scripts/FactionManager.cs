using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FactionData
{
    public FactionType faction;
    public List<Ship> ships = new List<Ship>();
    public string baseLocation;    // Main port or base of operations
    public int influence;          // Faction's influence in the Mediterranean
    public List<string> allies = new List<string>();
    public int resourceLevel;      // Economic resources available to the faction
}

public class FactionManager : MonoBehaviour
{
    private Dictionary<FactionType, FactionData> factions = new Dictionary<FactionType, FactionData>();
    private static FactionManager instance;

    public static FactionManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFactions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeFactions()
    {
        // Initialize each faction with historical bases and characteristics
        factions[FactionType.BarbaryPirates] = new FactionData { 
            faction = FactionType.BarbaryPirates,
            baseLocation = "Algiers",
            influence = 80,
            resourceLevel = 70
        };

        factions[FactionType.MalteseCorsairs] = new FactionData {
            faction = FactionType.MalteseCorsairs,
            baseLocation = "Valletta",
            influence = 60,
            resourceLevel = 65
        };

        factions[FactionType.UscocPirates] = new FactionData {
            faction = FactionType.UscocPirates,
            baseLocation = "Senj",
            influence = 40,
            resourceLevel = 30
        };

        factions[FactionType.LevanticPirates] = new FactionData {
            faction = FactionType.LevanticPirates,
            baseLocation = "Rhodes",
            influence = 50,
            resourceLevel = 45
        };

        // Initialize other factions with default values
        foreach (FactionType faction in System.Enum.GetValues(typeof(FactionType)))
        {
            if (!factions.ContainsKey(faction))
            {
                factions[faction] = new FactionData { 
                    faction = faction,
                    influence = 50,
                    resourceLevel = 50
                };
            }
        }
    }

    public void RegisterShip(FactionType faction, Ship ship)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            if (!factionData.ships.Contains(ship))
            {
                factionData.ships.Add(ship);
                Debug.Log($"Registered ship {ship.shipName} to faction {faction}");
            }
        }
        else
        {
            Debug.LogError($"Attempting to register ship for unknown faction: {faction}");
        }
    }

    public void UnregisterShip(FactionType faction, Ship ship)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            if (factionData.ships.Contains(ship))
            {
                factionData.ships.Remove(ship);
                Debug.Log($"Unregistered ship {ship.shipName} from faction {faction}");
            }
        }
        else
        {
            Debug.LogError($"Attempting to unregister ship from unknown faction: {faction}");
        }
    }

    public List<Ship> GetFactionShips(FactionType faction)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            return new List<Ship>(factionData.ships); // Return a copy to prevent external modification
        }
        else
        {
            Debug.LogError($"Attempting to get ships for unknown faction: {faction}");
            return new List<Ship>();
        }
    }

    public void UpdateFactionInfluence(FactionType faction, int change)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            factionData.influence = Mathf.Clamp(factionData.influence + change, 0, 100);
            Debug.Log($"Updated {faction} influence to {factionData.influence}");
        }
    }

    public FactionData GetFactionData(FactionType faction)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            return factionData;
        }
        Debug.LogError($"Attempting to get data for unknown faction: {faction}");
        return null;
    }

    public int GetFactionInfluence(FactionType faction)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            return factionData.influence;
        }
        return 0;
    }

    public int GetFactionResourceLevel(FactionType faction)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            return factionData.resourceLevel;
        }
        return 0;
    }
}
