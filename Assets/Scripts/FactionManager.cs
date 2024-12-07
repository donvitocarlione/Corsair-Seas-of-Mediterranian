using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FactionData
{
    public FactionType faction;
    public string name;
    public Color color = Color.white;
    public float reputation = 50f;
    public List<Ship> ships = new List<Ship>();
    public List<Pirate> pirates = new List<Pirate>();
    public List<Port> ports = new List<Port>();
    public string baseLocation;    // Main port or base of operations
    public int influence;          // Faction's influence in the Mediterranean
    public int resourceLevel;      // Economic resources available to the faction
    public Dictionary<FactionType, float> relations = new Dictionary<FactionType, float>();

    public void SetRelation(FactionType otherFaction, float value)
    {
        relations[otherFaction] = Mathf.Clamp(value, 0f, 100f);
    }

    public float GetRelation(FactionType otherFaction)
    {
        if (relations.TryGetValue(otherFaction, out float value))
        {
            return value;
        }
        return 50f; // Default neutral relation
    }
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
            name = "Barbary Pirates",
            baseLocation = "Algiers",
            influence = 80,
            resourceLevel = 70,
            color = new Color(0.8f, 0.2f, 0.2f) // Red
        };

        factions[FactionType.MalteseCorsairs] = new FactionData {
            faction = FactionType.MalteseCorsairs,
            name = "Maltese Corsairs",
            baseLocation = "Valletta",
            influence = 60,
            resourceLevel = 65,
            color = new Color(0.2f, 0.2f, 0.8f) // Blue
        };

        factions[FactionType.UscocPirates] = new FactionData {
            faction = FactionType.UscocPirates,
            name = "Uscoc Pirates",
            baseLocation = "Senj",
            influence = 40,
            resourceLevel = 30,
            color = new Color(0.2f, 0.8f, 0.2f) // Green
        };

        factions[FactionType.LevanticPirates] = new FactionData {
            faction = FactionType.LevanticPirates,
            name = "Levantic Pirates",
            baseLocation = "Rhodes",
            influence = 50,
            resourceLevel = 45,
            color = new Color(0.8f, 0.8f, 0.2f) // Yellow
        };

        // Initialize other factions with default values
        foreach (FactionType faction in System.Enum.GetValues(typeof(FactionType)))
        {
            if (!factions.ContainsKey(faction))
            {
                factions[faction] = new FactionData { 
                    faction = faction,
                    name = faction.ToString(),
                    influence = 50,
                    resourceLevel = 50,
                    color = Color.gray
                };
            }
        }

        // Initialize relations between factions
        foreach (var faction in factions.Values)
        {
            foreach (FactionType otherFaction in System.Enum.GetValues(typeof(FactionType)))
            {
                if (faction.faction != otherFaction)
                {
                    faction.SetRelation(otherFaction, 50f); // Default neutral relations
                }
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
