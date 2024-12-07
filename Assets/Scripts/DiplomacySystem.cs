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
        SetRelationship(FactionType.Federation, FactionType.Vulcans, Relationship.Friendly);
        SetRelationship(FactionType.Federation, FactionType.Andorians, Relationship.Friendly);
        
        // Traditional Rivalries
        SetRelationship(FactionType.Federation, FactionType.Klingons, Relationship.Hostile);
        SetRelationship(FactionType.Federation, FactionType.Romulans, Relationship.Hostile);
        SetRelationship(FactionType.Federation, FactionType.BorgCollective, Relationship.Hostile);
        
        // Commercial Relations
        SetRelationship(FactionType.Ferengi, FactionType.Federation, Relationship.Neutral);
        SetRelationship(FactionType.Ferengi, FactionType.Klingons, Relationship.Neutral);
        
        // Strategic Alliances
        SetRelationship(FactionType.Cardassians, FactionType.Dominion, Relationship.Friendly);
        SetRelationship(FactionType.Romulans, FactionType.Cardassians, Relationship.Friendly);
        
        // Isolationist Stances
        SetRelationship(FactionType.TholianAssembly, FactionType.Federation, Relationship.Hostile);
        SetRelationship(FactionType.TholianAssembly, FactionType.Klingons, Relationship.Hostile);
        
        // The Borg vs Everyone
        SetRelationship(FactionType.BorgCollective, FactionType.Klingons, Relationship.Hostile);
        SetRelationship(FactionType.BorgCollective, FactionType.Romulans, Relationship.Hostile);
        SetRelationship(FactionType.BorgCollective, FactionType.Cardassians, Relationship.Hostile);
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
