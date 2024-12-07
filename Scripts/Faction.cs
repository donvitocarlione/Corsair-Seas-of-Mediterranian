using System;
using System.Collections.Generic;
using UnityEngine;

public class Faction : MonoBehaviour
{
    public string factionName;
    public int influenceLevel = 0;
    public List<Pirate> members = new List<Pirate>();
    public List<Port> controlledPorts = new List<Port>();
    public Dictionary<Faction, int> relationships = new Dictionary<Faction, int>();

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

    public void SetRelationship(Faction otherFaction, int value)
    {
        // Clamp relationship value between -100 and 100
        value = Mathf.Clamp(value, -100, 100);
        relationships[otherFaction] = value;
    }

    public int GetRelationship(Faction otherFaction)
    {
        if (relationships.ContainsKey(otherFaction))
        {
            return relationships[otherFaction];
        }
        return 0;
    }
}
