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
        // Major Political Alignments
        SetRelationship(FactionType.Venetians, FactionType.Spanish, Relationship.Friendly);
        SetRelationship(FactionType.Venetians, FactionType.Knights, Relationship.Friendly);
        
        // Traditional Rivalries
        SetRelationship(FactionType.Ottoman, FactionType.Venetians, Relationship.Hostile);
        SetRelationship(FactionType.Ottoman, FactionType.Spanish, Relationship.Hostile);
        SetRelationship(FactionType.Ottoman, FactionType.Knights, Relationship.Hostile);
        
        // Commercial Relations
        SetRelationship(FactionType.Merchants, FactionType.Venetians, Relationship.Neutral);
        SetRelationship(FactionType.Merchants, FactionType.Ottoman, Relationship.Neutral);
        
        // Strategic Alliances
        SetRelationship(FactionType.Barbary, FactionType.Ottoman, Relationship.Friendly);
        SetRelationship(FactionType.Spanish, FactionType.Knights, Relationship.Friendly);
        
        // Pirate Relations
        SetRelationship(FactionType.Pirates, FactionType.Merchants, Relationship.Hostile);
        SetRelationship(FactionType.Pirates, FactionType.Venetians, Relationship.Hostile);
        
        // Genoese Relations
        SetRelationship(FactionType.Genoese, FactionType.Ottoman, Relationship.Neutral);
        SetRelationship(FactionType.Genoese, FactionType.Venetians, Relationship.Hostile);
        SetRelationship(FactionType.Genoese, FactionType.Spanish, Relationship.Neutral);
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