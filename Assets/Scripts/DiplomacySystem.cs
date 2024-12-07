using UnityEngine;
using System.Collections.Generic;

public class DiplomacySystem : MonoBehaviour
{
    private static DiplomacySystem instance;
    public static DiplomacySystem Instance => instance;

    public enum Relationship
    {
        Hostile = -1,
        Neutral = 0,
        Friendly = 1
    }

    [System.Serializable]
    public class FactionRelationship
    {
        public FactionType factionA;
        public FactionType factionB;
        public Relationship initialRelationship;
    }

    public FactionRelationship[] defaultRelationships;

    private Dictionary<(FactionType, FactionType), Relationship> relationships = new Dictionary<(FactionType, FactionType), Relationship>();
    private Dictionary<(FactionType, FactionType), int> relationshipValues = new Dictionary<(FactionType, FactionType), int>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitializeRelationships();
    }

    private void InitializeRelationships()
    {
        foreach (var rel in defaultRelationships)
        {
            SetRelationship(rel.factionA, rel.factionB, rel.initialRelationship);
            // Initialize relationship values (-100 to 100)
            SetRelationshipValue(rel.factionA, rel.factionB, (int)rel.initialRelationship * 50);
        }
    }

    public void SetRelationship(FactionType factionA, FactionType factionB, Relationship relationship)
    {
        relationships[(factionA, factionB)] = relationship;
        relationships[(factionB, factionA)] = relationship;
    }

    public Relationship GetRelationship(FactionType factionA, FactionType factionB)
    {
        if (relationships.TryGetValue((factionA, factionB), out Relationship relationship))
        {
            return relationship;
        }
        return Relationship.Neutral;
    }

    public void SetRelationshipValue(FactionType factionA, FactionType factionB, int value)
    {
        value = Mathf.Clamp(value, -100, 100);
        relationshipValues[(factionA, factionB)] = value;
        relationshipValues[(factionB, factionA)] = value;

        // Update relationship type based on value
        Relationship newRelationship = value <= -33 ? Relationship.Hostile :
                                     value >= 33 ? Relationship.Friendly :
                                     Relationship.Neutral;

        SetRelationship(factionA, factionB, newRelationship);
    }

    public int GetRelationshipValue(FactionType factionA, FactionType factionB)
    {
        if (relationshipValues.TryGetValue((factionA, factionB), out int value))
        {
            return value;
        }
        return 0;
    }

    public void ModifyRelationship(FactionType factionA, FactionType factionB, int change)
    {
        int currentValue = GetRelationshipValue(factionA, factionB);
        SetRelationshipValue(factionA, factionB, currentValue + change);
    }

    public bool AreFriendly(FactionType factionA, FactionType factionB)
    {
        return GetRelationship(factionA, factionB) == Relationship.Friendly;
    }

    public bool AreHostile(FactionType factionA, FactionType factionB)
    {
        return GetRelationship(factionA, factionB) == Relationship.Hostile;
    }
}
