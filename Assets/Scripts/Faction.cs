using System.Collections.Generic;
using UnityEngine;

public class Faction : MonoBehaviour
{
    public FactionType type;
    public string factionName;
    public int influenceLevel = 0;
    public List<Pirate> members = new List<Pirate>();
    public List<Port> controlledPorts = new List<Port>();

    void Start()
    {
        if (string.IsNullOrEmpty(factionName))
        {
            factionName = type.ToString();
        }
    }

    public void AddMember(Pirate pirate)
    {
        if (!members.Contains(pirate))
        {
            members.Add(pirate);
            pirate.SetFaction(this);
        }
    }

    public void RemoveMember(Pirate pirate)
    {
        if (members.Contains(pirate))
        {
            members.Remove(pirate);
            pirate.SetFaction(null);
        }
    }

    public int GetRelationshipWith(Faction otherFaction)
    {
        return DiplomacySystem.Instance.GetRelationshipValue(type, otherFaction.type);
    }

    public void ModifyRelationshipWith(Faction otherFaction, int change)
    {
        DiplomacySystem.Instance.ModifyRelationship(type, otherFaction.type, change);
    }

    public bool IsFriendlyWith(Faction otherFaction)
    {
        return DiplomacySystem.Instance.AreFriendly(type, otherFaction.type);
    }

    public bool IsHostileWith(Faction otherFaction)
    {
        return DiplomacySystem.Instance.AreHostile(type, otherFaction.type);
    }
}
