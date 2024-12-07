using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour
{
    public string pirateName;
    public List<Ship> ships = new List<Ship>();
    public Faction faction;
    public float reputation;
    public int gold;
    public Ship selectedShip;

    public virtual void AddShip(Ship ship)  // Added virtual keyword here
    {
        if (!ships.Contains(ship))
        {
            ships.Add(ship);
            ship.SetOwner(this);
            
            if (ships.Count == 1)
            {
                SelectShip(ship);
            }
        }
    }

    public virtual void RemoveShip(Ship ship)
    {
        if (ships.Contains(ship))
        {
            ships.Remove(ship);
            ship.SetOwner(null);
            
            if (selectedShip == ship)
            {
                selectedShip = ships.Count > 0 ? ships[0] : null;
                if (selectedShip != null)
                {
                    SelectShip(selectedShip);
                }
            }
        }
    }

    public virtual bool SelectShip(Ship ship)
    {
        if (ships.Contains(ship))
        {
            if (selectedShip != null)
            {
                selectedShip.Deselect();
            }
            selectedShip = ship;
            return ship.Select();
        }
        return false;
    }

    public void SetFaction(Faction newFaction)
    {
        if (faction != null)
        {
            faction.RemoveMember(this);
        }
        faction = newFaction;
    }

    public void JoinFaction(Faction newFaction)
    {
        if (newFaction != null)
        {
            newFaction.AddMember(this);
        }
    }

    public void LeaveFaction()
    {
        if (faction != null)
        {
            faction.RemoveMember(this);
        }
    }
}