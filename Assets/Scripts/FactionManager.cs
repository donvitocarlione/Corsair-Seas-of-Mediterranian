using UnityEngine;
using System.Collections.Generic;

[System.Serializable] // This is important; it allows for using the member in the Inspector
public class FactionData
{
    public FactionType faction;
    public List<Ship> ships = new List<Ship>();
}

public class FactionManager : MonoBehaviour
{
    public Dictionary<FactionType, FactionData> factions = new Dictionary<FactionType, FactionData>();

    void Awake()
    
    {
        InitializeFactions();
    }

    void Start() {  } 

    public void InitializeFactions()
    {
        foreach (FactionType faction in System.Enum.GetValues(typeof(FactionType)))
        {
            factions[faction] = new FactionData { faction = faction };
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
}
