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
    public Dictionary<FactionType, FactionData> factions = new Dictionary<FactionType, FactionData>();
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
            factionData.ships.Add(ship);
        }
        else
        {
            Debug.LogError("Attempting to register ship for unknown faction: " + faction);
        }
    }

    public List<Ship> GetFactionShips(FactionType faction)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            return factionData.ships;
        }
        else
        {
            Debug.LogError("Attempting to get ships for unknown faction: " + faction);
            return new List<Ship>();
        }
    }

    public void UpdateFactionInfluence(FactionType faction, int change)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            factionData.influence = Mathf.Clamp(factionData.influence + change, 0, 100);
        }
    }

    public FactionData GetFactionData(FactionType faction)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            return factionData;
        }
        return null;
    }
}