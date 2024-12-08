using UnityEngine;
using System.Collections.Generic;
using System;

public class FactionManager : MonoBehaviour
{
    // ... [Previous code remains the same until InitializeDefaultFaction] ...

    private void InitializeDefaultFaction(FactionType faction)
    {
        var newFaction = new FactionDefinition(
            faction,
            faction.ToString()
        )
        {
            Influence = 50,
            ResourceLevel = 50,
            Color = Color.gray,
            BaseLocation = "Unknown"
        };

        factions[faction] = newFaction;
        InitializeFactionRelations(newFaction);
    }

    private void InitializeHistoricalFaction(
        FactionType type,
        string name,
        string baseLocation,
        int influence,
        int resourceLevel,
        Color color)
    {
        var faction = new FactionDefinition(type, name)
        {
            BaseLocation = baseLocation,
            Influence = influence,
            ResourceLevel = resourceLevel,
            Color = color
        };

        factions[type] = faction;
        InitializeFactionRelations(faction);
    }

    private void InitializeFactionRelations(FactionDefinition faction)
    {
        foreach (FactionType otherFaction in Enum.GetValues(typeof(FactionType)))
        {
            if (faction.Type != otherFaction)
            {
                faction.SetRelation(otherFaction, 50f); // Default neutral relations
            }
        }
    }

    public void RegisterShip(FactionType faction, Ship ship)
    {
        if (factions.TryGetValue(faction, out FactionDefinition factionData))
        {
            factionData.AddShip(ship);
            OnShipRegistered?.Invoke(faction, ship);
            Debug.Log($"Registered ship {ship.ShipName} to faction {faction}");
        }
        else
        {
            Debug.LogError($"Attempting to register ship for unknown faction: {faction}");
        }
    }

    public void UnregisterShip(FactionType faction, Ship ship)
    {
        if (factions.TryGetValue(faction, out FactionDefinition factionData))
        {
            factionData.RemoveShip(ship);
            OnShipUnregistered?.Invoke(faction, ship);
            Debug.Log($"Unregistered ship {ship.ShipName} from faction {faction}");
        }
    }

    public void UpdateFactionRelation(FactionType faction1, FactionType faction2, float newValue)
    {
        if (faction1 == faction2) return;

        var faction1Data = GetFactionData(faction1);
        var faction2Data = GetFactionData(faction2);

        if (faction1Data != null && faction2Data != null)
        {
            faction1Data.SetRelation(faction2, newValue);
            faction2Data.SetRelation(faction1, newValue);
            OnRelationChanged?.Invoke(faction1, faction2, newValue);
        }
    }

    public void ModifyFactionInfluence(FactionType faction, int change)
    {
        if (factions.TryGetValue(faction, out FactionDefinition factionData))
        {
            int oldInfluence = factionData.Influence;
            factionData.Influence = Mathf.Clamp(factionData.Influence + change, 0, 100);
            
            if (oldInfluence != factionData.Influence)
            {
                OnInfluenceChanged?.Invoke(faction, factionData.Influence);
                Debug.Log($"Updated {faction} influence to {factionData.Influence}");
            }
        }
    }

    public void RecordTradeBetweenFactions(FactionType faction1, FactionType faction2, float value)
    {
        if (faction1 == faction2) return;

        var faction1Data = GetFactionData(faction1);
        var faction2Data = GetFactionData(faction2);

        if (faction1Data != null && faction2Data != null)
        {
            // Record trade and update relations
            faction1Data.SetRelation(faction2, 
                Mathf.Min(faction1Data.GetRelation(faction2) + value * 0.1f, 100f));
            faction2Data.SetRelation(faction1, 
                Mathf.Min(faction2Data.GetRelation(faction1) + value * 0.1f, 100f));

            OnRelationChanged?.Invoke(faction1, faction2, faction1Data.GetRelation(faction2));
        }
    }

    public FactionDefinition GetFactionData(FactionType faction)
    {
        if (factions.TryGetValue(faction, out FactionDefinition factionData))
        {
            return factionData;
        }
        Debug.LogError($"Attempting to get data for unknown faction: {faction}");
        return null;
    }

    public bool AreFactionsAtWar(FactionType faction1, FactionType faction2)
    {
        var faction1Data = GetFactionData(faction1);
        return faction1Data != null && 
               faction1Data.GetRelation(faction2) < 25f;
    }

    public float GetRelationBetweenFactions(FactionType faction1, FactionType faction2)
    {
        var faction1Data = GetFactionData(faction1);
        return faction1Data?.GetRelation(faction2) ?? 50f;
    }

    public IReadOnlyList<Ship> GetFactionShips(FactionType faction)
    {
        var factionData = GetFactionData(faction);
        return factionData?.Ships ?? new List<Ship>().AsReadOnly();
    }

    public IReadOnlyList<Port> GetFactionPorts(FactionType faction)
    {
        var factionData = GetFactionData(faction);
        return factionData?.Ports ?? new List<Port>().AsReadOnly();
    }

    public Color GetFactionColor(FactionType faction)
    {
        var factionData = GetFactionData(faction);
        return factionData?.Color ?? Color.gray;
    }
}