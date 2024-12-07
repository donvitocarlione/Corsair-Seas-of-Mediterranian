using UnityEngine;
using System.Collections.Generic;

public class DiplomacySystem : MonoBehaviour
{
    public enum Relationship
    {
        Friendly,
        Neutral,
        Hostile
    }

    private Dictionary<(FactionType, FactionType), Relationship> relationships = new Dictionary<(FactionType, FactionType), Relationship>();

    void Start()
    {
        InitializeRelationships();
    }

    private void InitializeRelationships()
    {
        // Historical Alliances
        SetRelationship(FactionType.BarbaryPirates, FactionType.AlgerianTaifa, Relationship.Friendly);
        SetRelationship(FactionType.BarbaryPirates, FactionType.RenegadoPirates, Relationship.Friendly);
        
        // Traditional Rivalries
        SetRelationship(FactionType.BarbaryPirates, FactionType.MalteseCorsairs, Relationship.Hostile);
        SetRelationship(FactionType.LevanticPirates, FactionType.MalteseCorsairs, Relationship.Hostile);
        
        // Regional Competitions
        SetRelationship(FactionType.GreekPirates, FactionType.UscocPirates, Relationship.Neutral);
        SetRelationship(FactionType.CilicianPirates, FactionType.LevanticPirates, Relationship.Neutral);
        
        // Strategic Alliances
        SetRelationship(FactionType.BarbarianCorsairs, FactionType.RenegadoPirates, Relationship.Friendly);
        SetRelationship(FactionType.LevanticPirates, FactionType.GreekPirates, Relationship.Friendly);
        
        // Historical Enemies
        SetRelationship(FactionType.ProvencalPirates, FactionType.BarbaryPirates, Relationship.Hostile);
        SetRelationship(FactionType.MalteseCorsairs, FactionType.AlgerianTaifa, Relationship.Hostile);
    }

    public void SetRelationship(FactionType factionA, FactionType factionB, Relationship relationship)
    {
        relationships[(factionA, factionB)] = relationship;
        relationships[(factionB, factionA)] = relationship; // Ensure bidirectional relationship
    }

    public Relationship GetRelationship(FactionType factionA, FactionType factionB)
    {
        if (relationships.TryGetValue((factionA, factionB), out Relationship relationship))
        {
            return relationship;
        }
        return Relationship.Neutral; // Default to Neutral if relationship is not defined
    }
}