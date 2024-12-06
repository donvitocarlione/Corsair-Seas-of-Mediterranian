using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FactionData
{
    public FactionType faction;
    public List<Ship> ships = new List<Ship>();
}

public class FactionManager : MonoBehaviour
{
    public static FactionManager Instance { get; private set; }
    public Dictionary<FactionType, FactionData> factions = new Dictionary<FactionType, FactionData>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        InitializeFactions();
    }

    public void InitializeFactions()
    {
        foreach (FactionType faction in System.Enum.GetValues(typeof(FactionType)))
        {
            factions[faction] = new FactionData { faction = faction };
        }
    }

    public void SetupShip(Ship ship, FactionType faction, string shipName)
    {
        ship.Initialize(faction, shipName);
        RegisterShip(faction, ship);
    }

    public void RegisterShip(FactionType faction, Ship ship)
    {
        if (factions.TryGetValue(faction, out FactionData factionData))
        {
            if (!factionData.ships.Contains(ship))
            {
                factionData.ships.Add(ship);
            }
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